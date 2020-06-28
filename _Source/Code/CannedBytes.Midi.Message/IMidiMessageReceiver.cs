namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// An interface that can be used in a 'chain of responsibilities'.
    /// </summary>
    [ContractClass(typeof(MidiMessageReceiverContract))]
    public interface IMidiMessageReceiver
    {
        /// <summary>
        /// Receives a short midi message.
        /// </summary>
        /// <param name="message">The midi message. Must not be null.</param>
        /// <param name="timestamp">A timestamp.</param>
        void ShortMessage(MidiShortMessage message, long timestamp);

        /// <summary>
        /// Receives a long midi message.
        /// </summary>
        /// <param name="message">The midi message. Must not be null.</param>
        /// <param name="timestamp">A timestamp.</param>
        void LongMessage(MidiLongMessage message, long timestamp);
    }
}