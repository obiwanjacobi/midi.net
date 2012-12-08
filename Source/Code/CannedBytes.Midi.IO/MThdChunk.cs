using CannedBytes.Media.IO.SchemaAttributes;

namespace CannedBytes.Midi.IO
{
    [Chunk("MThd")]
    public class MThdChunk
    {
        public ushort Format { get; set; }

        public ushort NumberOfTracks { get; set; }

        public ushort TimeDivision { get; set; }
    }
}