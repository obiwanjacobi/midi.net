using System.ComponentModel.Composition.Hosting;
using System.IO;
using CannedBytes.ComponentModel.Composition;
using CannedBytes.Media.IO;
using CannedBytes.Media.IO.ChunkTypes.Midi;
using CannedBytes.Media.IO.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.IO.UnitTests
{
    static class Factory
    {
        public static ChunkFileContext CreateFileContextForReading(string filePath)
        {
            Assert.IsNotNull(filePath);
            Assert.IsTrue(File.Exists(filePath));

            var context = ChunkFileContext.OpenFrom(filePath);

            Assert.IsNotNull(context);

            context.CompositionContainer = CreateCompositionContextForReading();

            Assert.IsNotNull(context.ChunkFile);
            Assert.IsNotNull(context.ChunkFile.BaseStream);
            Assert.IsNotNull(context.CompositionContainer);

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