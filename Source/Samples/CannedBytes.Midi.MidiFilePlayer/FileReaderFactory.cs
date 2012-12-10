using System.ComponentModel.Composition.Hosting;
using CannedBytes.ComponentModel.Composition;
using CannedBytes.Media.IO;
using CannedBytes.Media.IO.ChunkTypes.Midi;
using CannedBytes.Media.IO.Services;

namespace CannedBytes.Midi.MidiFilePlayer
{
    internal class FileReaderFactory
    {
        public static FileChunkReader CreateReader(string filePath)
        {
            var context = CreateFileContextForReading(filePath);

            var reader = context.CompositionContainer.CreateFileChunkReader();

            return reader;
        }

        public static ChunkFileContext CreateFileContextForReading(string filePath)
        {
            var context = ChunkFileContext.OpenFrom(filePath);

            context.CompositionContainer = CreateCompositionContextForReading();

            return context;
        }

        public static CompositionContainer CreateCompositionContextForReading()
        {
            var factory = new CompositionContainerFactory();

            factory.AddMarkedTypesInAssembly(null, typeof(IFileChunkHandler));
            // add midi exports
            factory.AddMarkedTypesInAssembly(typeof(MTrkChunkHandler).Assembly, typeof(IFileChunkHandler));

            // note that Midi files use big endian.
            // and the chunks are not aligned.
            factory.AddTypes(
                typeof(BigEndianNumberReader),
                typeof(SizePrefixedStringReader),
                typeof(ChunkTypeFactory),
                typeof(FileChunkHandlerManager));

            var container = factory.CreateNew();

            var chunkFactory = container.GetService<ChunkTypeFactory>();
            // add midi chunks
            chunkFactory.AddChunksFrom(typeof(MTrkChunkHandler).Assembly, true);

            return container;
        }
    }
}