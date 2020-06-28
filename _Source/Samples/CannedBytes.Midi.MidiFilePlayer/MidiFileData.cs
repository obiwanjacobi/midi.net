using System.Collections.Generic;
using CannedBytes.Midi.IO;

namespace CannedBytes.Midi.MidiFilePlayer
{
    class MidiFileData
    {
        public MThdChunk Header;
        public IEnumerable<MTrkChunk> Tracks;
    }
}