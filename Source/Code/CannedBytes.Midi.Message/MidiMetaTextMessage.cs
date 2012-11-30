using System.Text;

namespace CannedBytes.Midi.Message
{
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
        public MidiMetaTextMessage(MidiMetaTypes type, byte[] data)
            : base(type, data)
        { }

        /// <summary>
        /// Gets the <see cref="P:Data"/> (UTF7) encoded as a string.
        /// </summary>
        public string Text
        {
            get { return Encoding.UTF7.GetString(Data); }
        }
    }
}