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

        /// <summary>
        /// Gets the length of the midi message in bytes.
        /// </summary>
        public int ByteLength { get; protected set; }
    }
}