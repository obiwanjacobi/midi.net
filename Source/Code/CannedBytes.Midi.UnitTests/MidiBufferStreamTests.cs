using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiBufferStreamTests
    {
        private MidiOutPort CreateInitialzedOutPort()
        {
            var port = MidiOutPortTests.CreateMidiOutPort();

            port.BufferManager.Initialize(2, 256);
            port.Open(0);

            return port;
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void BufferLength_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(port.BufferManager.BufferSize, buffer.Length);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Capacity_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual((long)port.BufferManager.BufferSize, buffer.Capacity);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void SetLength_RetrievedBuffer_DoesNotThrowException()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                buffer.SetLength(0);

                Assert.AreEqual((long)0, buffer.Length);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void CanSeek_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(true, buffer.CanSeek);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void CanRead_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(true, buffer.CanRead);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void CanWrite_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                Assert.AreEqual(true, buffer.CanWrite);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void BytesRecorded_SetAndGet_WillRetrieveCorrectValue()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                const long expected = 7777777;
                buffer.BytesRecorded = expected;

                Assert.AreEqual(expected, buffer.BytesRecorded);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Seek_ToEnd_LengthIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Length);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Seek_ToEnd_PositionIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Position);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        public void Write_ThenReadBackPos0_ValuesMatch()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.RetrieveBuffer();

                var expected = (byte)0xAB;
                buffer.WriteByte(expected);
                buffer.Position = 0; //rewind
                var actual = buffer.ReadByte();

                Assert.AreEqual(expected, actual);

                port.BufferManager.ReturnBuffer(buffer);
            }
        }
    }
}