using System.Collections.Generic;
using CannedBytes.Media.IO.SchemaAttributes;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.IO
{
    /// <summary>
    /// Represents the midi track chunk in a midi file.
    /// </summary>
    /// <remarks>Although this class is marked as a chunk, it will typically
    /// be instantiated by the <see cref="T:MTrkChunkHandler"/> only.</remarks>
    [Chunk("MTrk")]
    public class MTrkChunk
    {
        /// <summary>
        /// A list of midi events that make up the track.
        /// </summary>
        public IEnumerable<MidiFileEvent> Events { get; set; }
    }
}