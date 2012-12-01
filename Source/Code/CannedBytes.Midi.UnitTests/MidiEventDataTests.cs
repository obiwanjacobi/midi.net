using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiEventDataTests
    {
        private const int Value24 = 0x00654301;
        private const int Value32 = unchecked((int)0x84654301);

        [TestMethod]
        public void Data_InitializedWithMSB_ReturnsSameValue()
        {
            var eventData = new MidiEventData(Value32);

            Assert.AreEqual(Value32, eventData.Data);
        }

        [TestMethod]
        public void Data_InitializedWithMSB_ReturnsNonMaskedValue()
        {
            var eventData = new MidiEventData(Value32);

            Assert.AreEqual(Value32, eventData.Data);
        }

        [TestMethod]
        public void Length_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData(Value32);

            Assert.AreEqual(Value24, eventData.Length);
        }

        [TestMethod]
        public void Tempo_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData(Value32);

            Assert.AreEqual(Value24, eventData.Tempo);
        }

        [TestMethod]
        public void EventType_InitializedWithMSB_ReturnsEnumValue()
        {
            var eventData = new MidiEventData(Value32);

            Assert.AreEqual(MidiEventType.LongVersion, eventData.EventType);
        }

        [TestMethod]
        public void Data_SetWithMSB_ReturnsNonMaskedValue()
        {
            var eventData = new MidiEventData();
            eventData.Data = Value32;

            Assert.AreEqual(Value32, eventData.Data);
        }

        [TestMethod]
        public void Length_SetWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData();
            eventData.Length = Value32;

            Assert.AreEqual(Value24, eventData.Length);
        }

        [TestMethod]
        public void Tempo_SetWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData();
            eventData.Tempo = Value32;

            Assert.AreEqual(Value24, eventData.Tempo);
        }

        [TestMethod]
        public void EventType_SetWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData();
            eventData.EventType = (MidiEventType)Value32;

            Assert.AreEqual(MidiEventType.ShortTempo, eventData.EventType);
        }

        [TestMethod]
        public void EventType_LengthSetWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiEventData(Value24);
            eventData.Length = Value32;

            Assert.AreEqual(MidiEventType.ShortMessage, eventData.EventType);
        }

        [TestMethod]
        public void EventType_LengthSetWithMSB_DoesNotOverwrite()
        {
            var eventData = new MidiEventData();
            eventData.EventType = MidiEventType.ShortNop;
            eventData.Length = Value32;

            Assert.AreEqual(MidiEventType.ShortNop, eventData.EventType);
        }

        [TestMethod]
        public void EventType_TempoSetWithMSB_DoesNotOverwrite()
        {
            var eventData = new MidiEventData();
            eventData.EventType = MidiEventType.ShortNop;
            eventData.Tempo = Value32;

            Assert.AreEqual(MidiEventType.ShortNop, eventData.EventType);
        }

        [TestMethod]
        public void Length_EventTypeSet_DoesNotOverwrite()
        {
            var eventData = new MidiEventData();
            eventData.Length = Value24;
            eventData.EventType = MidiEventType.ShortNop;

            Assert.AreEqual(Value24, eventData.Length);
        }

        [TestMethod]
        public void Tempo_EventTypeSet_DoesNotOverwrite()
        {
            var eventData = new MidiEventData();
            eventData.Tempo = Value24;
            eventData.EventType = MidiEventType.ShortNop;

            Assert.AreEqual(Value24, eventData.Tempo);
        }
    }
}