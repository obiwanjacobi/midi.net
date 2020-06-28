namespace CannedBytes.Midi
{
    /// <summary>
    /// Implemented by a Midi Port that can callback to its client for notification.
    /// </summary>
    /// <remarks>This interface is a receiver interface.</remarks>
    public interface IMidiDataCallback
    {
        /// <summary>
        /// Callback on a long midi message.
        /// </summary>
        /// <param name="buffer">The buffer that is involved in the notification. Must not be null.</param>
        /// <param name="notificationType">The type of notification.</param>
        void LongData(MidiBufferStream buffer, MidiDataCallbackType notificationType);
    }
}