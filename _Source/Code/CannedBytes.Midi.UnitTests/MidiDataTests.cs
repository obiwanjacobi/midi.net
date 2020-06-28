using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiDataTests
    {
        private const byte StatusValue = 0x81;
        private const byte Param1Value = 0x43;
        private const byte Param2Value = 0x65;
        private const int RunningStatusDataValue = 0x6543;
        private const int Value24 = 0x00654381;
        private const int Value32 = unchecked((int)0x87654381);

        [TestMethod]
        public void Data_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiData(Value32);

            Assert.AreEqual(Value24, eventData.Data);
        }

        [TestMethod]
        public void Status_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiData(Value32);

            Assert.AreEqual(StatusValue, eventData.Status);
        }

        [TestMethod]
        public void Param1_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiData(Value32);

            Assert.AreEqual(Param1Value, eventData.Parameter1);
        }

        [TestMethod]
        public void Param2_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiData(Value32);

            Assert.AreEqual(Param2Value, eventData.Parameter2);
        }

        [TestMethod]
        public void RunningStatusData_InitializedWithMSB_ReturnsMaskedValue()
        {
            var eventData = new MidiData(Value32);

            Assert.AreEqual(RunningStatusDataValue, eventData.RunningStatusData);
        }

        [TestMethod]
        public void Status_Param1Set_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Parameter1 = Param1Value;

            Assert.AreEqual(StatusValue, eventData.Status);
        }

        [TestMethod]
        public void Status_Param2Set_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Parameter1 = Param2Value;

            Assert.AreEqual(StatusValue, eventData.Status);
        }

        [TestMethod]
        public void Param1_StatusSet_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Status = StatusValue;

            Assert.AreEqual(Param1Value, eventData.Parameter1);
        }

        [TestMethod]
        public void Param1_Param2Set_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Parameter2 = Param2Value;

            Assert.AreEqual(Param1Value, eventData.Parameter1);
        }

        [TestMethod]
        public void Param2_StatusSet_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Status = StatusValue;

            Assert.AreEqual(Param2Value, eventData.Parameter2);
        }

        [TestMethod]
        public void Param2_Param1Set_DoesNotOverwrite()
        {
            var eventData = new MidiData(Value32);
            eventData.Parameter1 = Param1Value;

            Assert.AreEqual(Param2Value, eventData.Parameter2);
        }
    }
}