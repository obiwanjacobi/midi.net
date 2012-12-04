namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A base class for midi messages that span more than a few (3) bytes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="CannedBytes.Midi.Messages.MidiSysExMessage"/>
    /// <seealso cref="CannedBytes.Midi.Messages.MidiMetaMessage"/>
    /// </remarks>
    public abstract class MidiLongMessage : MidiMessage
    {
        /// <summary>
        /// Implementation of the IMidiMessage.GetData method.
        /// </summary>
        /// <returns>Returns the <see cref="P:Data"/> property.</returns>
        public override byte[] GetData()
        {
            return Data;
        }

        private byte[] _data;

        /// <summary>
        /// Gets the long midi message data as a byte buffer.
        /// </summary>
        /// <remarks>Derived classes can set the property but value must not be null.</remarks>
        public byte[] Data
        {
            get { return _data; }
            protected set
            {
                //Contract.Requires<ArgumentNullException>(value != null);

                _data = value;
                ByteLength = value.Length;
            }
        }
    }
}