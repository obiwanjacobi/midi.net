namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// Enumerates the types of midi records that can be in the queue.
    /// </summary>
    public enum MidiQueueRecordType
    {
        /// <summary>An invalid value.</summary>
        Empty,
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