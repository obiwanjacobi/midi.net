using CannedBytes.Media.IO;
using System.Collections.Generic;
using System.IO;

namespace CannedBytes.Midi.IO
{
    public sealed class MidiFile
    {
        public MThdChunk Header { get; set; }
        public IEnumerable<MTrkChunk> Tracks { get; set; }

        public static MidiFile Read(string filePath)
        {
            Check.IfArgumentNullOrEmpty(filePath, nameof(filePath));

            var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForReading(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();

            return Read(context);
        }

        public static MidiFile Read(Stream stream)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForReading(stream)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();

            return Read(context);
        }

        public static MidiFile Read(ChunkFileContext context)
        {
            Check.IfArgumentNull(context, nameof(context));

            var reader = new FileChunkReader(context);
            var file = new MidiFile
            {
                Header = reader.ReadNextChunk() as MThdChunk
            };

            var tracks = new List<MTrkChunk>();
            for (int i = 0; i < file.Header.NumberOfTracks; i++)
            {
                var track = reader.ReadNextChunk() as MTrkChunk;
                tracks.Add(track);
            }

            file.Tracks = tracks;
            return file;
        }

        public static void Write(MidiFile midiFile, string filePath)
        {
            Check.IfArgumentNull(midiFile, nameof(midiFile));
            Check.IfArgumentNullOrEmpty(filePath, nameof(filePath));

            var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForWriting(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();

            Write(midiFile, context);
        }

        public static void Write(MidiFile midiFile, Stream stream)
        {
            Check.IfArgumentNull(midiFile, nameof(midiFile));
            Check.IfArgumentNull(stream, nameof(stream));

            var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForWriting(stream)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build();

            Write(midiFile, context);
        }

        public static void Write(MidiFile midiFile, ChunkFileContext context)
        {
            Check.IfArgumentNull(midiFile, nameof(midiFile));
            Check.IfArgumentNull(context, nameof(context));

            var writer = new FileChunkWriter(context);

            writer.WriteNextChunk(midiFile.Header);

            foreach (var track in midiFile.Tracks)
            {
                writer.WriteNextChunk(track);
            }
        }
    }
}
