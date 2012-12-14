namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The base class for all midi messages.
    /// </summary>
    public abstract class MidiMessage : IMidiMessage
    {
        /// <summary>
        /// Derived classes return the message data.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public abstract byte[] GetData();

        /// <summary>
        /// Backing field for the <see cref="ByteLength"/> property.
        /// </summary>
        private int byteLength;

        /// <summary>
        /// Gets the length of the midi message in bytes.
        /// </summary>
        public int ByteLength
        {
            get
            {
                return this.byteLength;
            }

            protected set
            {
                Contract.Requires(value > 0);
                Check.IfArgumentOutOfRange(value, 1, int.MaxValue, "ByteLength");

                this.byteLength = value;
            }
        }
    }
}