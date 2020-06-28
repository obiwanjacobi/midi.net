namespace CannedBytes.Midi.IO
{
    using CannedBytes.Media.IO.SchemaAttributes;

    /// <summary>
    /// Represents the Midi Header chunk in a midi file.
    /// </summary>
    [Chunk("MThd")]
    public class MThdChunk
    {
        /// <summary>
        /// The file format of the midi file.
        /// </summary>
        public ushort Format { get; set; }

        /// <summary>
        /// The number of tracks stored in the midi file.
        /// </summary>
        public ushort NumberOfTracks { get; set; }

        /// <summary>
        /// The time division used for timing of the midi events.
        /// </summary>
        public ushort TimeDivision { get; set; }
    }
}