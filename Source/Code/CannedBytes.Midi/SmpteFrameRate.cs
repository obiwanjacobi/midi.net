namespace CannedBytes.Midi
{
    /// <summary>
    /// Different frame rates for SMPTE.
    /// </summary>
    public enum SmpteFrameRate
    {
        /// <summary>Not set.</summary>
        None,

        /// <summary>24 frames per second.</summary>
        Smpte24,

        /// <summary>25 frames per second.</summary>
        Smpte25,

        /// <summary>Drop-30 frames per second.</summary>
        /// <remarks>Not supported!</remarks>
        SmpteDrop30,

        /// <summary>30 frames per second.</summary>
        Smpte30,
    }
}