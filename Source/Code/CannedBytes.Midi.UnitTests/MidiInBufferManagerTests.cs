using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiInBufferManagerTests
    {
        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_PortNotOpen_AllBuffersUnused()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.BufferManager.Initialize(2, 256);

                Assert.AreEqual(port.BufferManager.BufferCount, port.BufferManager.UnusedBufferCount);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_BuffersCreated_BufferCountMatches()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(2, port.BufferManager.BufferCount);

                port.Reset();
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_BuffersCreated_BufferSizeMatches()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(256, port.BufferManager.BufferSize);

                port.Reset();
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_OpenState_NoUnusedBuffers()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(0, port.BufferManager.UnusedBufferCount);

                port.Reset();
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_OpenState_AllBuffersUsed()
        {
            using (var port = MidiInPortTests.CreateMidiInPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(port.BufferManager.BufferCount, port.BufferManager.UsedBufferCount);

                port.Reset();
            }
        }
    }
}