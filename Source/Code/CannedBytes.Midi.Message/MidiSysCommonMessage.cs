namespace CannedBytes.Midi.Message
{
    public class MidiSysCommonMessage : MidiShortMessage
    {
        public MidiSysCommonMessage(int data)
        {
            Data = data;
            ByteLength = 1;
        }
    }
}