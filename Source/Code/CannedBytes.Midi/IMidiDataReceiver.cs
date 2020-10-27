namespace CannedBytes.Midi
{
    /// <summary>
    /// The IMidiReceiver interface is used to communicate
    /// received midi events down a receiver chain.
    /// </summary>
    public interface IMidiDataReceiver
    {
        /// <summary>
        /// A short midi message is received.
        /// </summary>
        /// <param name="data">The short midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        void ShortData(int data, long timestamp);

        /// <summary>
        /// A long midi message is received.
        /// </summary>
        /// <param name="stream">The long midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        void LongData(IMidiStream stream, long timestamp);
    }
}