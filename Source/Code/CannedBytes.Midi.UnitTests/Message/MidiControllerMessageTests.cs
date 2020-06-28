using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Message.UnitTests
{
    /// <summary>
    /// Unit Tests for the MidiControllerMessage class.
    /// </summary>
    [TestClass]
    public class MidiControllerMessageTests
    {
        private const int Message32Data = 0x77FF63B1;
        private const int MessageData = 0x00FF63B1;
        private const int InvalidMessageData = 0x00FF6381;
        private const int StatusData = 0xB1;
        private const int ControllerTypeData = 0x63;
        private const int ParamData = 0xFF;
        private const byte ChannelData = 0x01;

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ControllerType_OnInvalidMessage_ThrowsException()
        {
            var msg = new MidiControllerMessage(InvalidMessageData);

            Assert.Fail();
        }

        [TestMethod]
        public void ControllerType_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiControllerMessage(MessageData);

            Assert.AreEqual(MidiControllerType.NonregisteredParameterCoarse, msg.ControllerType);
        }

        [TestMethod]
        public void Param_OnInitializedMessage_ReturnsMaskedValue()
        {
            var msg = new MidiControllerMessage(Message32Data);

            Assert.AreEqual(ParamData, msg.Value);
        }

        [TestMethod]
        public void Status_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiControllerMessage(MessageData);

            Assert.AreEqual(StatusData, msg.Status);
        }

        [TestMethod]
        public void MidiChannel_OnInitializedMessage_ReturnsCorrectValue()
        {
            var msg = new MidiControllerMessage(MessageData);

            Assert.AreEqual(ChannelData, msg.MidiChannel);
        }
    }
}