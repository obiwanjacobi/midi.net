using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Message.UnitTests
{
    [TestClass]
    public class MidiMessageFactoryTests
    {
        private const int NoteMessage = 0x00FF6381;
        private const int RealtimeMessage = 0x000000FF;
        private const int InvalidRealtimeMessage = 0x000000FD;
        private const int CommonMessage = 0x000000F1;
        private const int InvalidCommonMessage = 0x000000F0;
        private const int ControllerMessage = 0x00FF07B1;

        [TestMethod]
        public void CreateShortMessage_NoteMessage_ReturnsNotNull()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(NoteMessage);

            Assert.IsNotNull(msg);
        }

        [TestMethod]
        public void CreateShortMessage_NoteMessage_ReturnsCorrectMessageType()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(NoteMessage);

            Assert.IsInstanceOfType(msg, typeof(MidiChannelMessage));
        }

        [TestMethod]
        public void CreateShortMessage_NoteMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiChannelMessage)factory.CreateShortMessage(NoteMessage);

            Assert.AreEqual(NoteMessage, msg.Data);
            Assert.AreEqual(0x81, msg.Status);
            Assert.AreEqual(0x01, msg.MidiChannel);
            Assert.AreEqual(0x63, msg.Param1);
            Assert.AreEqual(0xFF, msg.Param2);
        }

        [TestMethod]
        public void CreateShortMessage_PooledNoteMessage_ReturnsSameInstance()
        {
            var factory = new MidiMessageFactory();

            var msg1 = factory.CreateShortMessage(NoteMessage);
            var msg2 = factory.CreateShortMessage(NoteMessage);

            Assert.AreSame(msg1, msg2);
        }

        [TestMethod]
        public void CreateShortMessage_ControllerMessage_ReturnsCorrectMessageType()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(ControllerMessage);

            Assert.IsInstanceOfType(msg, typeof(MidiControllerMessage));
        }

        [TestMethod]
        public void CreateShortMessage_ControllerMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiControllerMessage)factory.CreateShortMessage(ControllerMessage);

            Assert.AreEqual(ControllerMessage, msg.Data);
            Assert.AreEqual(0xB1, msg.Status);
            Assert.AreEqual(0x07, msg.Param1);
            Assert.AreEqual(0xFF, msg.Param2);
            Assert.AreEqual(MidiControllerTypes.Volume, msg.ControllerType);
            Assert.AreEqual(0x01, msg.MidiChannel);
            Assert.AreEqual(0xFF, msg.Param);
        }

        [TestMethod]
        public void CreateShortMessage_SysCommonMessage_ReturnsCorrectMessageType()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(CommonMessage);

            Assert.IsInstanceOfType(msg, typeof(MidiSysCommonMessage));
        }

        [TestMethod]
        public void CreateShortMessage_SysCommonMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysCommonMessage)factory.CreateShortMessage(CommonMessage);

            Assert.AreEqual(CommonMessage, msg.Data);
            Assert.AreEqual(0xF1, msg.Status);
            Assert.AreEqual(0x00, msg.Param1);
            Assert.AreEqual(0x00, msg.Param2);
            Assert.AreEqual(MidiSysCommonTypes.MtcQuarterFrame, msg.CommonType);
        }

        [TestMethod]
        public void CreateShortMessage_InvalidSysCommonMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysCommonMessage)factory.CreateShortMessage(InvalidCommonMessage);

            Assert.AreEqual(InvalidCommonMessage, msg.Data);
            Assert.AreEqual(0xF0, msg.Status);
            Assert.AreEqual(0x00, msg.Param1);
            Assert.AreEqual(0x00, msg.Param2);
            Assert.AreEqual(MidiSysCommonTypes.Invalid, msg.CommonType);
        }

        [TestMethod]
        public void CreateShortMessage_SysRealtimeMessage_ReturnsCorrectMessageType()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(RealtimeMessage);

            Assert.IsInstanceOfType(msg, typeof(MidiSysRealtimeMessage));
        }

        [TestMethod]
        public void CreateShortMessage_SysRealtimeMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysRealtimeMessage)factory.CreateShortMessage(RealtimeMessage);

            Assert.AreEqual(RealtimeMessage, msg.Data);
            Assert.AreEqual(0xFF, msg.Status);
            Assert.AreEqual(0x00, msg.Param1);
            Assert.AreEqual(0x00, msg.Param2);
            Assert.AreEqual(MidiSysRealtimeTypes.Reset, msg.RealtimeType);
        }

        [TestMethod]
        public void CreateShortMessage_InvalidSysRealtimeMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysRealtimeMessage)factory.CreateShortMessage(InvalidRealtimeMessage);

            Assert.AreEqual(InvalidRealtimeMessage, msg.Data);
            Assert.AreEqual(0xFD, msg.Status);
            Assert.AreEqual(0x00, msg.Param1);
            Assert.AreEqual(0x00, msg.Param2);
            Assert.AreEqual(MidiSysRealtimeTypes.Invalid, msg.RealtimeType);
        }
    }
}