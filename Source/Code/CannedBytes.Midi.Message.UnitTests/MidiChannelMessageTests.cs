using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Message.UnitTests
{
    /// <summary>
    /// Unit Tests for the MidiChannelMessage class.
    /// </summary>
    [TestClass]
    public class MidiChannelMessageTests
    {
        private const int Message32Data = 0x77FF6381;
        private const int MessageData = 0x00FF6381;
        private const int StatusData = 0x81;
        private const int Param1Data = 0x63;
        private const int Param2Data = 0xFF;
        private const byte ChannelData = 0x01;

        [TestMethod]
        public void Data_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(MessageData, msg.Data);
        }

        [TestMethod]
        public void ByteLength_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(3, msg.ByteLength);
        }

        [TestMethod]
        public void Data_OnInitializedMessage_ReturnsMaskedValue()
        {
            var msg = new MidiChannelMessage(Message32Data);

            Assert.AreEqual(MessageData, msg.Data);
        }

        [TestMethod]
        public void Status_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(StatusData, msg.Status);
        }

        [TestMethod]
        public void Param1_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(Param1Data, msg.Param1);
        }

        [TestMethod]
        public void Param2_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(Param2Data, msg.Param2);
        }

        [TestMethod]
        public void Command_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(MidiChannelCommands.NoteOff, msg.Command);
        }

        [TestMethod]
        public void MidiChannel_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiChannelMessage(MessageData);

            Assert.AreEqual(ChannelData, msg.MidiChannel);
        }
    }
}