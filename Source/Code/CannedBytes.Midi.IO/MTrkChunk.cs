using System.Collections.Generic;
using CannedBytes.Media.IO.SchemaAttributes;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.IO
{
    [Chunk("MTrk")]
    public class MTrkChunk
    {
        public IEnumerable<MidiEvent> Events { get; set; }
    }
}