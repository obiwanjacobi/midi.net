using CannedBytes.Midi.Samples.SysExUtil.Midi;
using System.Collections.Generic;
using System.IO;

namespace CannedBytes.Midi.Samples.SysExUtil
{
    internal sealed class SysExSerializer
    {
        public void Serialize(Stream stream, IEnumerable<MidiSysExBuffer> buffers)
        {
            foreach (var buffer in buffers)
            {
                Serialize(stream, buffer);
            }
        }

        private void Serialize(Stream stream, MidiSysExBuffer buffer)
        {
            buffer.Stream.CopyTo(stream, 0);
        }

        public IEnumerable<MidiSysExBuffer> Deserialize(Stream stream)
        {
            var buffers = new List<MidiSysExBuffer>();
            var temp = new MemoryStream();

            while (ScanSysExMarkers(stream, temp))
            {
                var length = (int)temp.Position;
                var buffer = new MidiSysExBuffer(length);
                temp.Position = 0;  // re-read

                temp.CopyTo(buffer.Stream, length);
                buffers.Add(buffer);
                temp = new MemoryStream();
            }

            return buffers;
        }

        private bool ScanSysExMarkers(Stream stream, MemoryStream temp)
        {
            var writer = new BinaryWriter(temp);
            int value;

            // find start marker
            do
            {
                value = stream.ReadByte();
            }
            while (value != -1 && value != 0xF0);

            // end of stream
            if (value == -1) return false;

            // write start marker
            writer.Write((byte)value);

            // write until end marker
            while (value != -1 && value != 0xF7)
            {
                value = stream.ReadByte();
                writer.Write((byte)value);
            }

            return true;
        }
    }
}
