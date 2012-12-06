using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Message
{
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

        private int byteLength;

        /// <summary>
        /// Gets the length of the midi message in bytes.
        /// </summary>
        public int ByteLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == this.byteLength);

                return this.byteLength;
            }
            protected set
            {
                Contract.Requires(value > 0);
                Contract.Ensures(value == this.byteLength);
                Throw.IfArgumentOutOfRange(value, 1, int.MaxValue, "ByteLength");

                this.byteLength = value;
            }
        }
    }
}