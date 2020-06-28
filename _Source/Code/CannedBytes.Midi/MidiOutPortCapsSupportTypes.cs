namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// Indicates Midi Out driver support.
    /// </summary>
    [Flags]
    public enum MidiOutPortCapsSupportTypes
    {
        /// <summary>
        /// The driver supports volume control (MIDICAPS_VOLUME).
        /// </summary>
        Volume = 0x01,

        /// <summary>
        /// The driver supports separate left-right volume control (MIDICAPS_LRVOLUME).
        /// </summary>
        LeftRightVolume = 0x02,

        /// <summary>
        /// The driver supports patch caching (MIDICAPS_CACHE).
        /// </summary>
        PatchCaching = 0x04,

        /// <summary>
        /// The driver supports midiStreamOut directly (MIDICAPS_STREAM).
        /// </summary>
        Stream = 0x08,
    }
}