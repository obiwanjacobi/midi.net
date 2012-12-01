using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiBufferStreamTests
    {
        private MidiOutPort CreateInitialzedOutPort()
        {
            var port = MidiOutPortTests.CreateMidiOutPort();

            port.MidiBufferManager.Initialize(2, 256);
            port.Open(0);

            return port;
        }

        [TestMethod]
        public void BufferLength_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(port.MidiBufferManager.BufferSize, buffer.Length);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Capacity_RetrievedBuffer_ReflectsValueOnManager()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual((long)port.MidiBufferManager.BufferSize, buffer.Capacity);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void SetLength_RetrievedBuffer_DoesNotThrowException()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                buffer.SetLength(0);

                Assert.AreEqual((long)0, buffer.Length);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanSeek_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanSeek);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanRead_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanRead);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void CanWrite_RetrievedBuffer_True()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                Assert.AreEqual(true, buffer.CanWrite);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Seek_ToEnd_LengthIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Length);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Seek_ToEnd_PositionIsCapacity()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                buffer.Seek(0, System.IO.SeekOrigin.End);

                Assert.AreEqual(buffer.Capacity, buffer.Position);

                port.MidiBufferManager.Return(buffer);
            }
        }

        [TestMethod]
        public void Write_ThenReadBackPos0_ValuesMatch()
        {
            using (var port = CreateInitialzedOutPort())
            {
                var buffer = port.MidiBufferManager.Retrieve();

                var expected = (byte)0xAB;
                buffer.WriteByte(expected);
                buffer.Position = 0; //rewind
                var actual = buffer.ReadByte();

                Assert.AreEqual(expected, actual);

                port.MidiBufferManager.Return(buffer);
            }
        }
    }
}