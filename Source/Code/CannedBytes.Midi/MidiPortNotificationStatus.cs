namespace CannedBytes.Midi
{
    /// <summary>
    /// Enumerates the callback notification statuses.
    /// </summary>
    public enum MidiPortNotificationStatus
    {
        /// <summary>Invalid value.</summary>
        None,
        /// <summary>Port is done.</summary>
        Done,
        /// <summary>Callback in midi stream.</summary>
        Callback,
    }
}