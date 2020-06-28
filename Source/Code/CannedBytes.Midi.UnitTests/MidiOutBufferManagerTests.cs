using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiOutBufferManagerTests
    {
        public TestContext TestContext { get; set; }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_PortNotOpen_AllBuffersUnused()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);

                Assert.AreEqual(port.BufferManager.BufferCount, port.BufferManager.UnusedBufferCount);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_PortOpen_AllBuffersUnused()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(port.BufferManager.BufferCount, port.BufferManager.UnusedBufferCount);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_BuffersCreated_BufferCountMatches()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(2, port.BufferManager.BufferCount);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Initialize_BuffersCreated_BufferSizeMatches()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                Assert.AreEqual(256, port.BufferManager.BufferSize);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Dispose_WithBuffersInUse_ThrowsAnError()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.IsNotNull(buffer);

                // return buffer or we get an exception on Dispose
                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Retrieve_InitializedBufferManager_ReturnsNonNull()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.IsNotNull(buffer);

                // return buffer or we get an exception on Dispose
                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Retrieve_InitializedBufferManager_ReturnsCorrectNumberOfBuffers()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer1 = port.BufferManager.RetrieveBuffer();
                var buffer2 = port.BufferManager.RetrieveBuffer();
                var buffer3 = port.BufferManager.RetrieveBuffer();

                Assert.IsNotNull(buffer1);
                Assert.IsNotNull(buffer2);
                Assert.IsNull(buffer3);

                // return buffer or we get an exception on Dispose
                port.BufferManager.ReturnBuffer(buffer1);
                port.BufferManager.ReturnBuffer(buffer2);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Retrieve_InitializedBufferManager_OneLessUsuableBuffer()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(1, port.BufferManager.UsedBufferCount);

                // return buffer or we get an exception on Dispose
                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Retrieve_InitializedBufferManager_OneMoreUnusuableBuffer()
        {
            using (var port = MidiOutPortTests.CreateMidiOutPort())
            {
                port.BufferManager.Initialize(2, 256);
                port.Open(0);

                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(1, port.BufferManager.UnusedBufferCount);

                // return buffer or we get an exception on Dispose
                port.BufferManager.ReturnBuffer(buffer);
            }
        }
    }
}