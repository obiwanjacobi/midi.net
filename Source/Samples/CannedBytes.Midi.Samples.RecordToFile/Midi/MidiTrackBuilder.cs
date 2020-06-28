using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System.Collections.Generic;
using System.Linq;

namespace CannedBytes.Midi.Samples.RecordToFile.Midi
{
    internal sealed class MidiTrackBuilder
    {
        private readonly IEnumerable<MidiFileEvent> _events;

        public MidiTrackBuilder(IEnumerable<MidiFileEvent> events)
        {
            _events = events;
        }

        public IEnumerable<MTrkChunk> Tracks { get; private set; }

        public void AddEndOfTrackMarkers()
        {
            if (Tracks != null)
            {
                foreach (var track in Tracks)
                {
                    var eof = new MidiFileEvent
                    {
                        Message = new MidiMetaMessage(MidiMetaType.EndOfTrack, new byte[] { })
                    };

                    var list = new List<MidiFileEvent>(track.Events);
                    var lastEvent = list[list.Count - 1];

                    eof.AbsoluteTime = lastEvent.AbsoluteTime + 1;
                    eof.DeltaTime = 1;

                    list.Add(eof);

                    track.Events = list;
                }
            }
        }

        public void BuildTracks()
        {
            var result = from fileEvent in _events
                         group fileEvent by (((MidiShortMessage)fileEvent.Message).Status & 0x0F) into trackGroups
                         orderby trackGroups.Key
                         select new MTrkChunk { Events = trackGroups };

            // fix the delta times
            foreach (var track in result)
            {
                MidiFileEvent lastEvent = null;

                foreach (var fileEvent in track.Events)
                {
                    if (lastEvent != null)
                    {
                        fileEvent.DeltaTime = fileEvent.AbsoluteTime - lastEvent.AbsoluteTime;
                    }
                    else
                    {
                        fileEvent.DeltaTime = 0;
                    }

                    lastEvent = fileEvent;
                }
            }

            Tracks = result.ToList();
        }
    }
}