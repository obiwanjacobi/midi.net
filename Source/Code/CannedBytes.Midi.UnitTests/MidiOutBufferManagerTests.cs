using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiOutBufferManagerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Initialize_PortNotOpen_AllBuffersUnused()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);

                Assert.AreEqual(port.MidiBufferManager.BufferCount, port.MidiBufferManager.UnusedBufferCount);
            }
        }

        [TestMethod]
        public void Initialize_PortOpen_AllBuffersUnused()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(port.MidiBufferManager.BufferCount, port.MidiBufferManager.UnusedBufferCount);
            }
        }
    }
}