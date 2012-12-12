namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Text;

    /// <summary>
    /// Represents a midi meta message with data that is text (string).
    /// </summary>
    /// <remarks>Note that meta messages only occur in midi files.</remarks>
    public class MidiMetaTextMessage : MidiMetaMessage
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        /// <param name="data">The data for the meta message.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Throw is not recognized.")]
        public MidiMetaTextMessage(MidiMetaType type, byte[] data)
            : base(type, data)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Length > 0);
        }

        /// <summary>
        /// Gets the <see cref="P:Data"/> (UTF7) encoded as a string.
        /// </summary>
        public string Text
        {
            get { return Encoding.UTF7.GetString(GetData()); }
        }
    }
}