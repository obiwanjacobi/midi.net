namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a System Exclusive midi message.
    /// </summary>
    public class MidiSysExMessage : MidiLongMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Must not be null or empty.</param>
        /// <param name="isContinuation">An indication if this message is a continuation on a previous message.</param>
        /// <remarks>The sysex markers are removed from the message <paramref name="data"/>.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public MidiSysExMessage(byte[] data, bool isContinuation)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Length > 0);
            Check.IfArgumentNull(data, "data");

            data = StripMarkers(data);
            SetData(data);

            IsContinuation = isContinuation;
        }

        /// <summary>
        /// Removes the sysex markers from the beginning and/or end of the message <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Must not be null.</param>
        /// <returns>Returns the data without the sysex markers.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected static byte[] StripMarkers(byte[] data)
        {
            Check.IfArgumentNull(data, "data");

            if (data.Length > 0)
            {
                int startIndex = 0;
                int newLength = data.Length;

                if (data[0] == 0xF0 || data[0] == 0xF7)
                {
                    startIndex = 1;
                }

                if (data[data.Length - 1] == 0xF7)
                {
                    newLength = data.Length - 1;
                }

                if (startIndex > 0 || newLength != data.Length)
                {
                    newLength -= startIndex;

                    var buffer = new byte[newLength];

                    for (int i = 0; i < newLength; i++)
                    {
                        buffer[i] = data[startIndex + i];
                    }

                    return buffer;
                }
            }

            return data;
        }

        /// <summary>
        /// Gets an indication if this message is a continuation on a previous message.
        /// </summary>
        public bool IsContinuation { get; protected set; }
    }
}