namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// The MidiPortStatus enumeration specifies the states a midi port can be in.
    /// </summary>
    /// <remarks>Note that not all states are valid for all types of Midi Ports.</remarks>
    [Flags]
    public enum MidiPortStatus
    {
        /// <summary>Status not specified (invalid status).</summary>
        None = 0x0000,

        /// <summary>The Midi Port is closed. No other status can be present.</summary>
        Closed = 0x0001,

        /// <summary>The Midi Port is opened.</summary>
        Open = 0x0002,

        /// <summary>The Midi Port is started. It can now record or play.</summary>
        Started = 0x0004,

        /// <summary>The Midi Port is stopped.</summary>
        Stopped = 0x0008,

        /// <summary>The Midi Port is paused.</summary>
        Paused = 0x0010,

        /// <summary>The Midi Port is reset. The reset flag is cleared at the next state change.</summary>
        Reset = 0x0020,

        /// <summary>Additional flag indicating an in-progress to a certain state.</summary>
        Pending = 0x0100,
    }
}