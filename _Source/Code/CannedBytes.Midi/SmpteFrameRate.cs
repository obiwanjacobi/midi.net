namespace CannedBytes.Midi
{
    /// <summary>
    /// Different frame rates for SMPTE.
    /// </summary>
    public enum SmpteFrameRate
    {
        /// <summary>Not set.</summary>
        None = 0,

        /// <summary>24 frames per second.</summary>
        Smpte24 = 24,

        /// <summary>25 frames per second.</summary>
        Smpte25 = 25,

        /// <summary>Drop-30 frames per second.</summary>
        /// <remarks>Not supported!</remarks>
        SmpteDrop30 = 29,

        /// <summary>30 frames per second.</summary>
        Smpte30 = 30,
    }
}