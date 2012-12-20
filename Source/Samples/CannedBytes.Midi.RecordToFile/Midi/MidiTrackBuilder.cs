using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.RecordToFile.Midi
{
    class MidiTrackBuilder
    {
        private IEnumerable<MidiFileEvent> events;

        public MidiTrackBuilder(IEnumerable<MidiFileEvent> events)
        {
            this.events = events;
        }

        public IEnumerable<MTrkChunk> Tracks { get; private set; }

        public void AddEndOfTrackMarkers()
        {
            if (this.Tracks != null)
            {
                foreach (var track in this.Tracks)
                {
                    var eof = new MidiFileEvent();
                    eof.Message = new MidiMetaMessage(MidiMetaType.EndOfTrack, new byte[] { });

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
            this.Tracks = from fileEvent in this.events
                          group fileEvent by (((MidiShortMessage)fileEvent.Message).Status & 0x0F) into trackGroups
                          orderby trackGroups.Key
                          select new MTrkChunk { Events = trackGroups };

            // fix the delta times
            foreach (var track in this.Tracks)
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
        }
    }
}