using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using CannedBytes.ComponentModel.Composition;
using CannedBytes.Media.IO;
using CannedBytes.Media.IO.Services;
using CannedBytes.Midi.IO;

namespace CannedBytes.Midi.RecordToFile.Midi
{
    class MidiFileSerializer : DisposableBase
    {
        private ChunkFileContext context = new ChunkFileContext();

        public MidiFileSerializer(string filePath)
        {
            this.context.ChunkFile = ChunkFileInfo.OpenWrite(filePath);
            this.context.CompositionContainer = CreateCompositionContainer();
        }

        private CompositionContainer CreateCompositionContainer()
        {
            var factory = new CompositionContainerFactory();
            var midiIOAssembly = typeof(MTrkChunkHandler).Assembly;

            // add basic file handlers
            factory.AddMarkedTypesInAssembly(null, typeof(IFileChunkHandler));

            // add midi file handlers
            factory.AddMarkedTypesInAssembly(midiIOAssembly, typeof(IFileChunkHandler));

            // note that Midi files use big endian.
            // and the chunks are not aligned.
            factory.AddTypes(
                typeof(BigEndianNumberWriter),
                typeof(SizePrefixedStringWriter),
                typeof(ChunkTypeFactory),
                typeof(FileChunkHandlerManager));

            var container = factory.CreateNew();

            var chunkFactory = container.GetService<ChunkTypeFactory>();

            // add midi chunks
            chunkFactory.AddChunksFrom(midiIOAssembly, true);

            return container;
        }

        public void Serialize(IEnumerable<MidiFileEvent> events)
        {
            var builder = new MidiTrackBuilder(events);
            builder.BuildTracks();
            builder.AddEndOfTrackMarkers();

            var header = new MThdChunk();
            header.Format = (ushort)MidiFileFormat.MultipleTracks;
            header.NumberOfTracks = (ushort)builder.Tracks.Count();
            header.TimeDivision = 408;

            var writer = new FileChunkWriter(this.context);

            writer.WriteNextChunk(header);

            foreach (var trackChunk in builder.Tracks)
            {
                writer.WriteNextChunk(trackChunk);
            }
        }

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.context.Dispose();
        }
    }
}
