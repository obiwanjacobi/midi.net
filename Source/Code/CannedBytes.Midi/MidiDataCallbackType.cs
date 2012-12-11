namespace CannedBytes.Midi
{
    /// <summary>
    /// Enumerates the types of notifications.
    /// </summary>
    public enum MidiDataCallbackType
    {
        /// <summary>
        /// No reason / unknown reason.
        /// </summary>
        Unknown,

        /// <summary>
        /// There was a callback from the port.
        /// </summary>
        Notification,

        /// <summary>
        /// Processing (of the buffer) is done.
        /// </summary>
        Done,

        /// <summary>
        /// There was an error.
        /// </summary>
        Error,
    }
}