namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents Common System (short) midi message.
    /// </summary>
    public class MidiSysCommonMessage : MidiShortMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified message <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Only the least significant byte is set.</param>
        public MidiSysCommonMessage(int data)
        {
            Data = data;
            ByteLength = 1;
        }
    }
}