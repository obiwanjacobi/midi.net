using CannedBytes.Media.IO;
using FluentAssertions;
using System.IO;

namespace CannedBytes.Midi.IO.UnitTests
{
    static class Factory
    {
        public static ChunkFileContext CreateFileContextForReading(string filePath)
        {
            filePath.Should().NotBeNullOrEmpty();
            File.Exists(filePath).Should().BeTrue();

            return new ChunkFileContextBuilder()
                .BigEndian()
                .ForReading(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();
        }

        public static ChunkFileContext CreateFileContextForWriting(string filePath)
        {
            filePath.Should().NotBeNullOrEmpty();

            return new ChunkFileContextBuilder()
                .BigEndian()
                .ForWriting(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();
        }
    }
}