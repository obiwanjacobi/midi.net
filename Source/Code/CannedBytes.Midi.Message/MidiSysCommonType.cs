namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The different types of system common mid messages.
    /// </summary>
    public enum MidiSysCommonType
    {
        /// <summary>
        /// Not a midi system common type.
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// Some master device that controls sequence playback sends this
        /// timing message to keep a slave device in sync with the master.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mtc", Justification = "Midi terminology.")]
        MtcQuarterFrame = 0xF1,

        /// <summary>
        /// Some master device that controls sequence playback sends this message to
        /// force a slave device to cue the playback to a certain point in the song/sequence.
        /// In other words, this message sets the device's "Song Position". This message
        /// doesn't actually start the playback. It just sets up the device to be
        /// "ready to play" at a particular point in the song.
        /// </summary>
        SongPositionPointer = 0xF2,

        /// <summary>
        /// Some master device that controls sequence playback sends this message to
        /// force a slave device to set a certain song for playback (ie, sequencing).
        /// </summary>
        SongSelect = 0xF3,

        /// <summary>
        /// The device receiving this should perform a tuning calibration.
        /// </summary>
        TuneRequest = 0xF4,
    }
}