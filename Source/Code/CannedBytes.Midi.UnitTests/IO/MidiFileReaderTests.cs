using CannedBytes.Media.IO;
using CannedBytes.Midi.IO.UnitTests.Media;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace CannedBytes.Midi.IO.UnitTests
{
    [TestClass]
    [DeploymentItem(@"IO\Media\town.mid")]
    public class MidiFileReaderTests
    {
        public TestContext TestContext { get; set; }

        internal static FileChunkReader CreateReader(string filePath)
        {
            var context = Factory.CreateFileContextForReading(filePath);
            var reader = new FileChunkReader(context);
            return reader;
        }

        [TestMethod]
        public void ReadChunk_ReadsToEnd_MidFile()
        {
            var filePath = Path.Combine(TestContext.DeploymentDirectory, TestMedia.MidFileName);
            var reader = CreateReader(filePath);

            // because there is no root chunk, we need to call ReadNextChunk multiple times.
            var midiHdr = reader.ReadNextChunk() as MThdChunk;
            midiHdr.Should().NotBeNull();

            var tracks = new List<MTrkChunk>();

            for (int i = 0; i < midiHdr.NumberOfTracks; i++)
            {
                var track = reader.ReadNextChunk() as MTrkChunk;
                track.Should().NotBeNull();

                tracks.Add(track);
            }
        }
    }
}