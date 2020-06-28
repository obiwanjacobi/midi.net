using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CannedBytes.Midi.SysExUtil.Midi;
using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.SysExUtil.Persistence
{
    class SysExSerializer
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
            StreamHelpers.CopyTo(buffer.Stream, stream, 0);
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

                StreamHelpers.CopyTo(temp, buffer.Stream, length);
                //temp.CopyTo(buffer.Stream);


                buffers.Add(buffer);

                temp = new MemoryStream();
            }

            return buffers;
        }

        private bool ScanSysExMarkers(Stream stream, MemoryStream temp)
        {
            var writer = new BinaryWriter(temp);

            int value = 0;
            
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
