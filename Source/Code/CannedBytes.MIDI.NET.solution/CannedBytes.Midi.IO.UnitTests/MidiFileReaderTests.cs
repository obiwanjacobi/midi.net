using System.Collections.Generic;
using System.IO;
using CannedBytes.Media.IO;
using CannedBytes.Midi.IO.UnitTests.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.IO.UnitTests
{
    [TestClass]
    [DeploymentItem(@"Media\town.mid")]
    public class MidiFileReaderTests
    {
        public TestContext TestContext { get; set; }

        internal static FileChunkReader CreateReader(string filePath)
        {
            var context = Factory.CreateFileContextForReading(filePath);
            Assert.IsNotNull(context);

            var reader = context.CompositionContainer.CreateFileChunkReader();

            Assert.IsNotNull(reader);

            return reader;
        }

        [TestMethod]
        public void ReadChunk_ReadsToEnd_MidFile()
        {
            var filePath = Path.Combine(TestContext.DeploymentDirectory, TestMedia.MidFileName);
            var reader = CreateReader(filePath);

            // because there is no root chunk, we need to call ReadNextChunk multiple times.
            var midiHdr = reader.ReadNextChunk() as MThdChunk;
            Assert.IsNotNull(midiHdr);

            var tracks = new List<MTrkChunk>();

            for (int i = 0; i < midiHdr.NumberOfTracks; i++)
            {
                var track = reader.ReadNextChunk() as MTrkChunk;
                Assert.IsNotNull(track);

                tracks.Add(track);
            }
        }
    }
}