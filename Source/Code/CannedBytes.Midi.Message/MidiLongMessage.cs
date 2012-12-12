namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A base class for midi messages that span more than a few (3) bytes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="T:MidiSysExMessage"/>
    /// <seealso cref="T:MidiMetaMessage"/>
    /// </remarks>
    public abstract class MidiLongMessage : MidiMessage
    {
        /// <summary>
        /// Implementation of the IMidiMessage.GetData method.
        /// </summary>
        /// <returns>Returns the <see cref="P:Data"/> property.</returns>
        public override byte[] GetData()
        {
            return this.data;
        }

        /// <summary>
        /// Maintains the data of the long message.
        /// </summary>
        private byte[] data;

        /// <summary>
        /// Returns the long midi message data as a byte buffer.
        /// </summary>
        /// <param name="value">The data to be set. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Throw is not recognized.")]
        protected void SetData(byte[] value)
        {
            Contract.Requires(value != null);
            Contract.Requires(value.Length > 0);
            Contract.Ensures(ByteLength == value.Length);
            Throw.IfArgumentNull(value, "value");

            this.data = value;
            ByteLength = value.Length;
        }
    }
}