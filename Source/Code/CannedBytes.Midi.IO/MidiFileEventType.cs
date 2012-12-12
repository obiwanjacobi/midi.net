namespace CannedBytes.Midi.IO
{
    /// <summary>
    /// The types of events that can be found in a midi file.
    /// </summary>
    public enum MidiFileEventType
    {
        /// <summary>
        /// Not initialized / unknown.
        /// </summary>
        None,

        /// <summary>
        /// Short midi event.
        /// </summary>
        Event,

        /// <summary>
        /// A System Exclusive message.
        /// </summary>
        SystemExclusive,

        /// <summary>
        /// A System Exclusive message continuation.
        /// </summary>
        SystemExclusiveContinuation,

        /// <summary>
        /// A meta event.
        /// </summary>
        Meta,
    }
}