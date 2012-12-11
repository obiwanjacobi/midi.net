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

        [TestMethod]
        public void BufferLength_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                Assert.AreEqual(port.BufferManager.BufferSize, buffer.Length);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Capacity_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                Assert.AreEqual((long)port.BufferManager.BufferSize, buffer.Capacity);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void SetLength_RetrievedBuffer_DoesNotThrowException()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                buffer.SetLength(0);

                Assert.AreEqual((long)0, buffer.Length);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanSeek_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanSeek);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanRead_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanRead);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanWrite_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanWrite);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Seek_ToEnd_LengthIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Length);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Seek_ToEnd_PositionIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Position);

                port.BufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Write_ThenReadBackPos0_ValuesMatch()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.BufferManager.Retrieve();

                var expected = (byte)0xAB;
                buffer.WriteByte(expected);
                buffer.Position = 0; //rewind
                var actual = buffer.ReadByte();

                Assert.AreEqual(expected, actual);

                port.BufferManager.Return(buffer);
            }
        }
    }
}