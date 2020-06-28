using CannedBytes.Midi.IO;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Samples.RecordToFile.Midi
{
    internal static class MidiFileSerializer
    {
        public static void Serialize(IEnumerable<MidiFileEvent> events, string filePath)
        {
            var builder = new MidiTrackBuilder(events);
            builder.BuildTracks();
            builder.AddEndOfTrackMarkers();

            var file = new MidiFile
            {
                Header = new MThdChunk
                {
                    Format = (ushort)MidiFileFormat.MultipleTracks,
                    NumberOfTracks = (ushort)builder.Tracks.Count(),
                    TimeDivision = 408
                },
                Tracks = builder.Tracks
            };

            MidiFile.Write(file, filePath);
        }
    }
}
