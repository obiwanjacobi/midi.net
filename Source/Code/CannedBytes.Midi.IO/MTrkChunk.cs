namespace CannedBytes.Midi.IO
{
    using CannedBytes.Media.IO.SchemaAttributes;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the midi track chunk in a midi file.
    /// </summary>
    /// <remarks>Although this class is marked as a chunk, it will typically
    /// be instantiated by the <see cref="T:MTrkChunkHandler"/> only.</remarks>
    [Chunk("MTrk")]
    public sealed class MTrkChunk
    {
        /// <summary>
        /// A list of midi events that make up the track.
        /// </summary>
        public IEnumerable<MidiFileEvent> Events { get; set; }
    }
}