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
        /// <param name="timeIndex">The time at which the message was received.</param>
        void ShortData(int data, int timeIndex);

        /// <summary>
        /// A long midi message is received.
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <param name="timeIndex">The time at which the message was received.</param>
        void LongData(MidiBufferStream buffer, int timeIndex);
    }

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
        /// <param name="timeIndex">The time at which the message was received.</param>
        void ShortError(int data, int timeIndex);

        /// <summary>
        /// An error on a long midi message is received.
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <param name="timeIndex">The time at which the message was received.</param>
        void LongError(MidiBufferStream buffer, int timeIndex);
    }
}