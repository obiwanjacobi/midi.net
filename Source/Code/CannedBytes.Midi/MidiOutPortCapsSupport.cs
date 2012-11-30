using System;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Indicates Midi Out driver support.
    /// </summary>
    [Flags]
    public enum MidiOutPortCapsSupport
    {
        /// <summary>
        /// The driver supports volume control (MIDICAPS_VOLUME).
        /// </summary>
        Volume = 1,
        /// <summary>
        /// The driver supports separate left-right volume control (MIDICAPS_LRVOLUME).
        /// </summary>
        LeftRightVolume = 2,
        /// <summary>
        /// The driver supports patch caching (MIDICAPS_CACHE).
        /// </summary>
        PatchCaching = 4,
        /// <summary>
        /// The driver supports midiStreamOut directly (MIDICAPS_STREAM).
        /// </summary>
        Stream = 8,
    }
}