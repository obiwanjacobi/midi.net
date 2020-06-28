﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual(0x63, msg.Parameter1);
            Assert.AreEqual(0xFF, msg.Parameter2);
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
            Assert.AreEqual(0x07, msg.Parameter1);
            Assert.AreEqual(0xFF, msg.Parameter2);
            Assert.AreEqual(MidiControllerType.Volume, msg.ControllerType);
            Assert.AreEqual(0x01, msg.MidiChannel);
            Assert.AreEqual(0xFF, msg.Value);
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
            Assert.AreEqual(0x00, msg.Parameter1);
            Assert.AreEqual(0x00, msg.Parameter2);
            Assert.AreEqual(MidiSysCommonType.MtcQuarterFrame, msg.CommonType);
        }

        [TestMethod]
        public void CreateShortMessage_InvalidSysCommonMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysCommonMessage)factory.CreateShortMessage(InvalidCommonMessage);

            Assert.AreEqual(InvalidCommonMessage, msg.Data);
            Assert.AreEqual(0xF0, msg.Status);
            Assert.AreEqual(0x00, msg.Parameter1);
            Assert.AreEqual(0x00, msg.Parameter2);
            Assert.AreEqual(MidiSysCommonType.Invalid, msg.CommonType);
        }

        [TestMethod]
        public void CreateShortMessage_SysRealtimeMessage_ReturnsCorrectMessageType()
        {
            var factory = new MidiMessageFactory();

            var msg = factory.CreateShortMessage(RealtimeMessage);

            Assert.IsInstanceOfType(msg, typeof(MidiSysRealTimeMessage));
        }

        [TestMethod]
        public void CreateShortMessage_SysRealtimeMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysRealTimeMessage)factory.CreateShortMessage(RealtimeMessage);

            Assert.AreEqual(RealtimeMessage, msg.Data);
            Assert.AreEqual(0xFF, msg.Status);
            Assert.AreEqual(0x00, msg.Parameter1);
            Assert.AreEqual(0x00, msg.Parameter2);
            Assert.AreEqual(MidiSysRealTimeType.Reset, msg.RealTimeType);
        }

        [TestMethod]
        public void CreateShortMessage_InvalidSysRealtimeMessage_ReturnsCorrectMessage()
        {
            var factory = new MidiMessageFactory();

            var msg = (MidiSysRealTimeMessage)factory.CreateShortMessage(InvalidRealtimeMessage);

            Assert.AreEqual(InvalidRealtimeMessage, msg.Data);
            Assert.AreEqual(0xFD, msg.Status);
            Assert.AreEqual(0x00, msg.Parameter1);
            Assert.AreEqual(0x00, msg.Parameter2);
            Assert.AreEqual(MidiSysRealTimeType.Invalid, msg.RealTimeType);
        }
    }
}