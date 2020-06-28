namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// An interface that can be used in a 'chain of responsibilities'.
    /// </summary>
    public interface IMidiMessageSender
    {
        /// <summary>
        /// Sends a short midi message.
        /// </summary>
        /// <param name="message">The midi message. Must not be null.</param>
        void Send(MidiShortMessage message);

        /// <summary>
        /// Sends a long midi message.
        /// </summary>
        /// <param name="message">The midi message. Must not be null.</param>
        void Send(MidiLongMessage message);
    }
}