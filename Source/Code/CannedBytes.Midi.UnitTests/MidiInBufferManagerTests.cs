using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiInBufferManagerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Initialize_PortNotOpen_AllBuffersUnused()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.MidiBufferManager.Initialize(2, 256);

                Assert.AreEqual(port.MidiBufferManager.BufferCount, port.MidiBufferManager.UnusedBufferCount);
            }
        }

        [TestMethod]
        public void Initialize_BuffersCreated_BufferCountMatches()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(2, port.MidiBufferManager.BufferCount);
            }
        }

        [TestMethod]
        public void Initialize_BuffersCreated_BufferSizeMatches()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(256, port.MidiBufferManager.BufferSize);
            }
        }

        [TestMethod]
        public void Initialize_OpenState_NoUnusedBuffers()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.IsTrue(port.MidiBufferManager.UnusedBufferCount == 0);
            }
        }

        [TestMethod]
        public void Initialize_OpenState_AllBuffersUsed()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(port.MidiBufferManager.BufferCount, port.MidiBufferManager.UsedBufferCount);
            }
        }
    }
}