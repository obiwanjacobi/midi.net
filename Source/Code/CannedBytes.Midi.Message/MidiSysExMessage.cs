namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a System Exclusive midi message.
    /// </summary>
    public class MidiSysExMessage : MidiLongMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Must not be null or empty.</param>
        public MidiSysExMessage(byte[] data)
        {
            Data = data;
        }
    }
}