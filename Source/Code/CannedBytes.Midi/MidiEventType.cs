namespace CannedBytes.Midi
{
    /// <summary>
    /// The types of stream event.
    /// </summary>
    public enum MidiEventType
    {
        /// <summary>Short midi message event.</summary>
        ShortMessage = 0x00,

        /// <summary>A tempo event.</summary>
        ShortTempo = 0x01,

        /// <summary>A "no operation" event.</summary>
        ShortNop = 0x02,

        /// <summary>A short midi message with callback event.</summary>
        ShortMessageCallback = 0x40,

        /// <summary>A tempo with callback event.</summary>
        ShortTempoCallback = 0x41,

        /// <summary>A "no operation" with callback event.</summary>
        ShortNopCallback = 0x42,

        /// <summary>A long midi message event.</summary>
        LongMessage = 0x80,

        /// <summary>A (long) comment event.</summary>
        LongComment = 0x82,

        /// <summary>A (long) version event.</summary>
        LongVersion = 0x84,

        /// <summary>A (long) midi message with callback event.</summary>
        LongMessageCallback = 0xC0,

        /// <summary>A (long) comment with callback event.</summary>
        LongCommentCallback = 0xC2,

        /// <summary>A (long) version with callback event.</summary>
        LongVersionCallback = 0xC4,
    }
}