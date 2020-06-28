namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a midi meta message.
    /// </summary>
    /// <remarks>Note that meta messages only occur in midi files.</remarks>
    public class MidiMetaMessage : MidiLongMessage
    {
        /// <summary>
        /// For derived classes.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        protected MidiMetaMessage(MidiMetaType type)
        {
            MetaType = type;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        /// <param name="data">The data for the meta message.</param>
        public MidiMetaMessage(MidiMetaType type, byte[] data)
        {
            Check.IfArgumentNull(data, nameof(data));

            MetaType = type;
            SetData(data);
        }

        /// <summary>
        /// Gets a value indicating the type of meta message.
        /// </summary>
        public MidiMetaType MetaType { get; private set; }
    }
}