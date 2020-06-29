using CannedBytes.Media.IO;
using System.Collections.Generic;
using System.IO;

namespace CannedBytes.Midi.IO
{
    /// <summary>
    /// Represent the Midi event data in a Midi file.
    /// </summary>
    /// <remarks>Uses the <see cref="CannedBytes.Media.IO"/> chunk-file mechanism.</remarks>
    public sealed class MidiFile
    {
        /// <summary>
        /// Header information.
        /// </summary>
        public MThdChunk Header { get; set; }

        /// <summary>
        /// The list of tracks in the Midi file.
        /// </summary>
        public IEnumerable<MTrkChunk> Tracks { get; set; }

        /// <summary>
        /// Read event data of a Midi file.
        /// </summary>
        /// <param name="filePath">Must not be null or empty.</param>
        /// <returns>Never returns null.</returns>
        /// <remarks>Closes the file when done. May throw exceptions.</remarks>
        public static MidiFile Read(string filePath)
        {
            Check.IfArgumentNullOrEmpty(filePath, nameof(filePath));

            using (var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForReading(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build())
            {
                return Read(context);
            }
        }

        /// <summary>
        /// Read event data of a Midi file stream.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        /// <returns>Never returns null.</returns>
        /// <remarks>Closes the file when done. May throw exceptions.</remarks>
        public static MidiFile Read(Stream stream)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            using (var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForReading(stream)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build())
            {
                return Read(context);
            }
        }

        /// <summary>
        /// Read event data of a Midi file prepared chunk-file context.
        /// </summary>
        /// <param name="context">Must not be null.</param>
        /// <returns>Never returns null.</returns>
        /// <remarks>May throw exceptions.</remarks>
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

        /// <summary>
        /// Write event data to a Midi file.
        /// </summary>
        /// <param name="midiFile">The Midi file data to write. Must not be null.</param>
        /// <param name="filePath">Must not be null or empty.</param>
        /// <remarks>May throw exceptions.</remarks>
        public static void Write(MidiFile midiFile, string filePath)
        {
            Check.IfArgumentNull(midiFile, nameof(midiFile));
            Check.IfArgumentNullOrEmpty(filePath, nameof(filePath));

            using (var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForWriting(filePath)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build())
            {
                Write(midiFile, context);
            }
        }

        /// <summary>
        /// Write event data to a Midi file stream.
        /// </summary>
        /// <param name="midiFile">The Midi file data to write. Must not be null.</param>
        /// <param name="stream">Must not be null or empty.</param>
        /// <remarks>May throw exceptions.</remarks>
        public static void Write(MidiFile midiFile, Stream stream)
        {
            Check.IfArgumentNull(midiFile, nameof(midiFile));
            Check.IfArgumentNull(stream, nameof(stream));

            using (var context = new ChunkFileContextBuilder()
                .BigEndian()
                .ForWriting(stream)
                .Discover(typeof(MTrkChunkHandler).Assembly)
                .Build())
            {
                Write(midiFile, context);
            }
        }

        /// <summary>
        /// Write event data to a Midi file using a prepared chunk-file context.
        /// </summary>
        /// <param name="midiFile">The Midi file data to write. Must not be null.</param>
        /// <param name="context">Must not be null or empty.</param>
        /// <remarks>May throw exceptions.</remarks>
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
