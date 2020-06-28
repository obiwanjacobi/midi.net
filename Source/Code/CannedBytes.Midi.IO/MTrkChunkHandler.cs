namespace CannedBytes.Midi.IO
{
    using CannedBytes.Media.IO;
    using CannedBytes.Media.IO.SchemaAttributes;
    using CannedBytes.Midi.Message;
    using System.Collections.Generic;

    /// <summary>
    /// A custom chunk handler for the track chunk in a midi file.
    /// </summary>
    /// <remarks>It reads the midi track chunk data and fills a <see cref="MTrkChunk"/> instance.</remarks>
    [FileChunkHandler("MTrk")]
    public class MTrkChunkHandler : FileChunkHandler
    {
        /// <summary>
        /// A midi message factory for pooled short midi messages.
        /// </summary>
        private readonly MidiMessageFactory _midiMessageFactory = new MidiMessageFactory();

        /// <summary>
        /// Reads the midi track from the midi file.
        /// </summary>
        /// <param name="context">File context of the midi file being read. Must not be null.</param>
        /// <returns>Returns the custom chunk object containing the data that was read.</returns>
        public override object Read(ChunkFileContext context)
        {
            Check.IfArgumentNull(context, nameof(context));

            var reader = context.Services.GetService<FileChunkReader>();
            var stream = reader.CurrentStream;
            var chunk = new MTrkChunk();
            var events = new List<MidiFileEvent>();
            chunk.Events = events;

            var midiReader = new MidiFileStreamReader(stream);

            // use the indication to copy buffers.
            _midiMessageFactory.CopyData = context.CopyStreams;

            while (midiReader.ReadNextEvent())
            {
                MidiFileEvent midiEvent = null;

                switch (midiReader.EventType)
                {
                    case MidiFileEventType.Event:
                        midiEvent = CreateMidiEvent(midiReader.AbsoluteTime, midiReader.DeltaTime, midiReader.MidiEvent);
                        break;
                    case MidiFileEventType.SystemExclusive:
                        midiEvent = CreateSysExEvent(midiReader.AbsoluteTime, midiReader.DeltaTime, midiReader.Data, false);
                        break;
                    case MidiFileEventType.SystemExclusiveContinuation:
                        midiEvent = CreateSysExEvent(midiReader.AbsoluteTime, midiReader.DeltaTime, midiReader.Data, true);
                        break;
                    case MidiFileEventType.Meta:
                        midiEvent = CreateMetaEvent(midiReader.AbsoluteTime, midiReader.DeltaTime, midiReader.MetaEvent, midiReader.Data);
                        break;
                }

                if (midiEvent != null)
                {
                    events.Add(midiEvent);
                }
            }

            return chunk;
        }

        /// <summary>
        /// Creates a new midi event that represents a meta event.
        /// </summary>
        /// <param name="absoluteTime">The absolute-time of the event.</param>
        /// <param name="deltaTime">The delta-time of the event.</param>
        /// <param name="metaType">The type of meta event.</param>
        /// <param name="data">The data for the meta event.</param>
        /// <returns>Never returns null.</returns>
        private MidiFileEvent CreateMetaEvent(long absoluteTime, long deltaTime, byte metaType, byte[] data)
        {
            var midiEvent = new MidiFileEvent
            {
                AbsoluteTime = absoluteTime,
                DeltaTime = deltaTime,
                Message = _midiMessageFactory.CreateMetaMessage((MidiMetaType)metaType, data)
            };

            return midiEvent;
        }

        /// <summary>
        /// Creates a new midi event that represents a midi event.
        /// </summary>
        /// <param name="absoluteTime">The absolute-time of the event.</param>
        /// <param name="deltaTime">The delta-time of the event.</param>
        /// <param name="midiMsg">The midi event data.</param>
        /// <returns>Never returns null.</returns>
        private MidiFileEvent CreateMidiEvent(long absoluteTime, long deltaTime, int midiMsg)
        {
            var midiEvent = new MidiFileEvent
            {
                AbsoluteTime = absoluteTime,
                DeltaTime = deltaTime,
                Message = _midiMessageFactory.CreateShortMessage(midiMsg)
            };

            return midiEvent;
        }

        /// <summary>
        /// Creates a new midi event that represents a system exclusive event.
        /// </summary>
        /// <param name="absoluteTime">The absolute-time of the event.</param>
        /// <param name="deltaTime">The delta-time of the event.</param>
        /// <param name="data">The data for the sysex event.</param>
        /// <param name="isContinuation">An indication if the sysex data is a continuation on a previous message.</param>
        /// <returns>Never returns null.</returns>
        private MidiFileEvent CreateSysExEvent(long absoluteTime, long deltaTime, byte[] data, bool isContinuation)
        {
            var midiEvent = new MidiFileEvent
            {
                AbsoluteTime = absoluteTime,
                DeltaTime = deltaTime,
                Message = _midiMessageFactory.CreateSysExMessage(data, isContinuation)
            };

            return midiEvent;
        }

        /// <summary>
        /// Indicates if the chunk <paramref name="instance"/> can be written.
        /// </summary>
        /// <param name="instance">Must be a <see cref="MTrkChunk"/>.</param>
        /// <returns>Returns true if there is a good chance <see cref="Write"/> will
        /// successfully write the <paramref name="instance"/> to the file stream.</returns>
        public override bool CanWrite(object instance)
        {
            return base.CanWrite(instance) && instance is MTrkChunk;
        }

        /// <summary>
        /// Writes the chunk object <paramref name="instance"/> to the stream.
        /// </summary>
        /// <param name="context">Must not be null.</param>
        /// <param name="instance">Must not be null.</param>
        public override void Write(ChunkFileContext context, object instance)
        {
            Check.IfArgumentNull(context, nameof(context));
            Check.IfArgumentNull(instance, nameof(instance));
            Check.IfArgumentNotOfType<MTrkChunk>(instance, nameof(instance));

            var trackChunk = (MTrkChunk)instance;
            var writer = context.Services.GetService<FileChunkWriter>();
            var stream = writer.CurrentStream;
            var midiWriter = new MidiFileStreamWriter(stream);

            foreach (var message in trackChunk.Events)
            {
                if (message.Message is MidiShortMessage shortMessage)
                {
                    midiWriter.WriteMidiEvent(message.DeltaTime, shortMessage.Data);
                }
                else
                {
                    if (message.Message is MidiMetaMessage metaMsg)
                    {
                        midiWriter.WriteMetaEvent(message.DeltaTime, metaMsg.MetaType, metaMsg.GetData());
                    }
                    if (message.Message is MidiSysExMessage sysexMsg)
                    {
                        midiWriter.WriteSysExEvent(message.DeltaTime, sysexMsg.GetData(), sysexMsg.IsContinuation);
                    }
                }
            }
        }
    }
}