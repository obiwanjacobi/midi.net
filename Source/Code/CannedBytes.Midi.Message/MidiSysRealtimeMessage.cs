namespace CannedBytes.Midi.Message
{
    public class MidiSysRealtimeMessage : MidiShortMessage
    {
        public MidiSysRealtimeMessage(int data)
        {
            Data = data;
            ByteLength = 1;
        }
    }
}