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

        [TestMethod]
        public void Initialize_BuffersCreated_BufferCountMatches()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(2, port.MidiBufferManager.BufferCount);
            }
        }

        [TestMethod]
        public void Initialize_BuffersCreated_BufferSizeMatches()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(256, port.MidiBufferManager.BufferSize);
            }
        }

        [TestMethod]
        public void Dispose_WithBuffersInUse_ThrowsAnError()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.MidiBufferManager.Retrieve();

                Assert.IsNotNull(buffer);

                // return buffer or we get an exception on Dispose
                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Retrieve_InitializeBufferManager_ReturnsNonNull()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.MidiBufferManager.Retrieve();

                Assert.IsNotNull(buffer);

                // return buffer or we get an exception on Dispose
                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Retrieve_InitializeBufferManager_ReturnsCorrectNumberOfBuffers()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer1 = port.MidiBufferManager.Retrieve();
                var buffer2 = port.MidiBufferManager.Retrieve();
                var buffer3 = port.MidiBufferManager.Retrieve();

                Assert.IsNotNull(buffer1);
                Assert.IsNotNull(buffer2);
                Assert.IsNull(buffer3);

                // return buffer or we get an exception on Dispose
                port.MidiBufferManager.Return(buffer1);
                port.MidiBufferManager.Return(buffer2);
            }
        }

        [TestMethod]
        public void Retrieve_InitializeBufferManager_OneLessUsuableBuffer()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(1, port.MidiBufferManager.UsedBufferCount);

                // return buffer or we get an exception on Dispose
                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Retrieve_InitializeBufferManager_OneMoreUnusuableBuffer()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.MidiBufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(1, port.MidiBufferManager.UnusedBufferCount);

                // return buffer or we get an exception on Dispose
                port.MidiBufferManager.Return(buffer);
            }
        }
    }
}