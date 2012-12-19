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

            var context = new ChunkFileContext();
            context.ChunkFile = ChunkFileInfo.OpenRead(filePath);

            Assert.IsNotNull(context.ChunkFile);
            Assert.IsNotNull(context.ChunkFile.BaseStream);

            context.CompositionContainer = CreateCompositionContext();

            Assert.IsNotNull(context.CompositionContainer);

            return context;
        }

        public static ChunkFileContext CreateFileContextForWriting(string filePath)
        {
            Assert.IsNotNull(filePath);
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(filePath)));

            var context = new ChunkFileContext();
            context.ChunkFile = ChunkFileInfo.OpenWrite(filePath);

            Assert.IsNotNull(context.ChunkFile);
            Assert.IsNotNull(context.ChunkFile.BaseStream);

            context.CompositionContainer = CreateCompositionContext();

            Assert.IsNotNull(context.CompositionContainer);

            return context;
        }

        public static CompositionContainer CreateCompositionContext()
        {
            var factory = new CompositionContainerFactory();

            factory.AddMarkedTypesInAssembly(null, typeof(IFileChunkHandler));
            // add midi exports
            factory.AddMarkedTypesInAssembly(typeof(MTrkChunkHandler).Assembly, typeof(IFileChunkHandler));

            // note that Midi files use big endian.
            // and the chunks are not aligned.
            factory.AddTypes(
                typeof(BigEndianNumberReader), typeof(BigEndianNumberWriter),
                typeof(SizePrefixedStringReader), typeof(SizePrefixedStringWriter),
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