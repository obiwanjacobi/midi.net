namespace CannedBytes.Midi.Message
{
    using System;
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Base class validates.")]
        public MidiMetaTextMessage(MidiMetaType type, byte[] data)
            : base(type, data)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Length > 0);
            ThrowIfMetaTypeIsNotText(type);
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="type">The type of meta message.</param>
        /// <param name="text">The text for the meta message.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Base class validates.")]
        public MidiMetaTextMessage(MidiMetaType type, string text)
            : base(type)
        {
            Contract.Requires(text != null);
            Contract.Requires(text.Length > 0);
            Check.IfArgumentNull(text, "text");
            ThrowIfMetaTypeIsNotText(type);

            SetData(Encoding.UTF7.GetBytes(text));
        }

        /// <summary>
        /// Gets the <see cref="P:Data"/> (UTF7) encoded as a string.
        /// </summary>
        public string Text
        {
            get { return Encoding.UTF7.GetString(GetData()); }
        }

        /// <summary>
        /// Returns true when the <paramref name="type"/> is Text.
        /// </summary>
        /// <param name="type">The meta type.</param>
        /// <returns>Returns true if Text, otherwise false.</returns>
        public static bool IsMetaTextType(MidiMetaType type)
        {
            switch (type)
            {
                case MidiMetaType.Copyright:
                case MidiMetaType.CuePoint:
                case MidiMetaType.Custom:
                case MidiMetaType.DeviceName:
                case MidiMetaType.Instrument:
                case MidiMetaType.Lyric:
                case MidiMetaType.Marker:
                case MidiMetaType.PatchName:
                case MidiMetaType.Text:
                case MidiMetaType.TrackName:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when the meta type is not Text.
        /// </summary>
        /// <param name="type">The meta type.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="type"/> is not Text.</exception>
        private static void ThrowIfMetaTypeIsNotText(MidiMetaType type)
        {
            if (!IsMetaTextType(type))
            {
                throw new ArgumentException("The MidiMetaType specified is not of a Text message.");
            }
        }
    }
}