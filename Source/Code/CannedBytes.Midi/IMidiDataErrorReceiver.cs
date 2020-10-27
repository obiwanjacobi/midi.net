namespace CannedBytes.Midi
{
    /// <summary>
    /// The IMidiErrorReciever interface is used to communicate
    /// midi receive errors down a error-receiver chain.
    /// </summary>
    public interface IMidiDataErrorReceiver
    {
        /// <summary>
        /// An error on a short midi message is received.
        /// </summary>
        /// <param name="data">The short midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        void ShortError(int data, long timestamp);

        /// <summary>
        /// An error on a long midi message is received.
        /// </summary>
        /// <param name="stream">The long midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        void LongError(IMidiStream stream, long timestamp);
    }
}