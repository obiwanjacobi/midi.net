namespace CannedBytes.Midi.Message
{
    public class MidiSysExMessage : MidiLongMessage
    {
        public MidiSysExMessage(byte[] data)
        {
            Data = data;
        }
    }
}