using System.Collections.Generic;
using System.IO;
using CannedBytes.Media.IO;
using CannedBytes.Midi.IO.UnitTests.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.IO.UnitTests
{
    [TestClass]
    [DeploymentItem(@"Media\town.mid")]
    public class MidiFileWriterTests
    {
        public TestContext TestContext { get; set; }

        internal static FileChunkWriter CreateWriter(string filePath)
        {
            var context = Factory.CreateFileContextForWriting(filePath);
            Assert.IsNotNull(context);

            var writer = new FileChunkWriter(context);

            Assert.IsNotNull(writer);

            return writer;
        }

        [TestMethod]
        public void WriteMidiFile_CompareWithOriginal_NoDiffs()
        {
            var readerFilePath = Path.Combine(TestContext.DeploymentDirectory, TestMedia.MidFileName);
            var reader = MidiFileReaderTests.CreateReader(readerFilePath);

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

            // now write the file back out
            var writerFilePath = Path.Combine(TestContext.DeploymentDirectory,
                Path.GetFileNameWithoutExtension(TestMedia.MidFileName) + "_out" + Path.GetExtension(TestMedia.MidFileName));
            var writer = CreateWriter(writerFilePath);

            writer.WriteNextChunk(midiHdr);

            foreach (var track in tracks)
            {
                writer.WriteNextChunk(track);
            }
        }
    }
}