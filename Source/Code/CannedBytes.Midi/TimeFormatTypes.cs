namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// Enumerates the different time format types.
    /// </summary>
    [Flags]
    public enum TimeFormatTypes
    {
        /// <summary>Invalid value.</summary>
        None = 0x0000,

        /// <summary>Time in milliseconds.</summary>
        Milliseconds = 0x0001,

        /// <summary>Number of waveform-audio samples.</summary>
        Samples = 0x0002,

        /// <summary>Current byte offset from beginning of the file.</summary>
        Bytes = 0x0004,

        /// <summary>Society of Motion Picture and Television Engineers (SMPTE) time.</summary>
        Smpte = 0x0008,

        /// <summary>MIDI time.</summary>
        Midi = 0x0010,

        /// <summary>Ticks within a MIDI stream.</summary>
        Ticks = 0x0020
    }
}