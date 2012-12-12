namespace CannedBytes.Midi
{
    /// <summary>
    /// All the different types of events that a Midi Port can source.
    /// </summary>
    public enum MidiPortEventType
    {
        /// <summary>An invalid value.</summary>
        None,

        /// <summary>Record contains a short midi message.</summary>
        ShortData,

        /// <summary>Record contains a short midi message error.</summary>
        ShortError,

        /// <summary>Record contains another short midi message.</summary>
        MoreData,

        /// <summary>Record contains a long midi message.</summary>
        LongData,

        /// <summary>Record contains a long midi message error.</summary>
        LongError,
    }
}