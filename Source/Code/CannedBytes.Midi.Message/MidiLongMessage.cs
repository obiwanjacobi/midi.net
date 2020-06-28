namespace CannedBytes.Midi.Message
{
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
            return _data;
        }

        /// <summary>
        /// Maintains the data of the long message.
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// Returns the long midi message data as a byte buffer.
        /// </summary>
        /// <param name="value">The data to be set. Must not be null.</param>
        protected void SetData(byte[] value)
        {
            Check.IfArgumentNull(value, nameof(value));

            _data = value;
            ByteLength = value.Length;
        }
    }
}