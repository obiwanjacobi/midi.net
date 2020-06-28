namespace CannedBytes.Midi.IO
{
    /// <summary>
    /// The different file formats for midi files.
    /// </summary>
    public enum MidiFileFormat
    {
        /// <summary>
        /// The midi file contains a single track.
        /// </summary>
        SingleTrack = 0,

        /// <summary>
        /// The midi file contains multiple tracks.
        /// </summary>
        MultipleTracks = 1,

        /// <summary>
        /// The midi file contains multiple songs.
        /// </summary>
        MultiplePatterns = 2,
    }
}