namespace CannedBytes.Midi.IO
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using CannedBytes.Media.IO.SchemaAttributes;

    /// <summary>
    /// Represents the midi track chunk in a midi file.
    /// </summary>
    /// <remarks>Although this class is marked as a chunk, it will typically
    /// be instantiated by the <see cref="T:MTrkChunkHandler"/> only.</remarks>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Trk", Justification = "Literal chunk name is used.")]
    [Chunk("MTrk")]
    public class MTrkChunk
    {
        /// <summary>
        /// A list of midi events that make up the track.
        /// </summary>
        public IEnumerable<MidiFileEvent> Events { get; set; }
    }
}