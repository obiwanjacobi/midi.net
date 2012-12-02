namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a midi meta message.
    /// </summary>
    /// <remarks>Note that meta messages only occur in midi files.</remarks>
    public class MidiMetaMessage : MidiLongMessage
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        /// <param name="data">The data for the meta message.</param>
        public MidiMetaMessage(MidiMetaTypes type, byte[] data)
        {
            MetaType = type;
            Data = data;
        }

        /// <summary>
        /// Gets a value indicating the type of meta message.
        /// </summary>
        public MidiMetaTypes MetaType { get; private set; }
    }
}