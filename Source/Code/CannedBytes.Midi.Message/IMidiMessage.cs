namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A basic interface to a midi message.
    /// </summary>
    public interface IMidiMessage
    {
        /// <summary>
        /// The length of the message in bytes.
        /// </summary>
        int ByteLength { get; }

        /// <summary>
        /// Retrieves the message as a byte array.
        /// </summary>
        /// <returns>Never returns null. Do not modify the returned array.</returns>
        byte[] GetData();
    }
}