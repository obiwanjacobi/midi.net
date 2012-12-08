using System.Collections.Generic;
using CannedBytes.Media.IO.SchemaAttributes;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace CannedBytes.Media.IO.ChunkTypes.Midi
{
    [FileChunkHandler("MTrk")]
    public class MTrkChunkHandler : FileChunkHandler
    {
        MidiMessageFactory midiMessageFactory = new MidiMessageFactory();

        public override object Read(ChunkFileContext context)
        {
            var reader = context.CompositionContainer.GetService<FileChunkReader>();
            var stream = reader.CurrentStream;
            var chunk = new MTrkChunk();
            var events = new List<MidiEvent>();
            chunk.Events = events;

            var midiReader = new MidiFileStreamReader(stream);

            while (midiReader.ReadNextEvent())
            {
                MidiEvent midiEvent = null;

                switch (midiReader.EventType)
                {
                    case MidiFileEventType.Event:
                        midiEvent = CreateMidiEvent(midiReader.DeltaTime, midiReader.MidiEvent);
                        break;
                    case MidiFileEventType.SysEx:
                        midiEvent = CreateSysExEvent(midiReader.DeltaTime, midiReader.Data, false);
                        break;
                    case MidiFileEventType.SysExCont:
                        midiEvent = CreateSysExEvent(midiReader.DeltaTime, midiReader.Data, true);
                        break;
                    case MidiFileEventType.Meta:
                        midiEvent = CreateMetaEvent(midiReader.DeltaTime, midiReader.MetaEvent, midiReader.Data);
                        break;
                }

                if (midiEvent != null)
                {
                    events.Add(midiEvent);
                }
            }

            return chunk;
        }

        private MidiEvent CreateMetaEvent(long deltaTime, byte metaType, byte[] data)
        {
            var midiEvent = new MidiEvent();

            midiEvent.DeltaTime = deltaTime;
            midiEvent.Message = this.midiMessageFactory.CreateMetaMessage((MidiMetaTypes)metaType, data);

            return midiEvent;
        }

        private MidiEvent CreateMidiEvent(long deltaTime, int midiMsg)
        {
            var midiEvent = new MidiEvent();

            midiEvent.DeltaTime = deltaTime;
            midiEvent.Message = this.midiMessageFactory.CreateShortMessage(midiMsg);

            return midiEvent;
        }

        private MidiEvent CreateSysExEvent(long deltaTime, byte[] data, bool isContuation)
        {
            var midiEvent = new MidiEvent();

            midiEvent.DeltaTime = deltaTime;
            midiEvent.Message = this.midiMessageFactory.CreateSysExMessage(data);

            return midiEvent;
        }

        public override void Write(ChunkFileContext context, object instance)
        {
            throw new System.NotImplementedException();
        }
    }
}