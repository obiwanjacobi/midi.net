namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

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
            this.MetaType = type;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        /// <param name="data">The data for the meta message.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Check is not recognized.")]
        public MidiMetaMessage(MidiMetaType type, byte[] data)
        {
            Contract.Requires(data != null);
            Check.IfArgumentNull(data, "data");

            this.MetaType = type;
            SetData(data);
        }

        /// <summary>
        /// Gets a value indicating the type of meta message.
        /// </summary>
        public MidiMetaType MetaType { get; private set; }
    }
}