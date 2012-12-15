using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CannedBytes.IO;

namespace CannedBytes.Midi.SysExUtil.Midi
{
    class MidiSysExBuffer
    {
        public const char ByteSeperator = ' ';

        private byte[] buffer;

        public MidiSysExBuffer(int capacity)
        {
            this.buffer = new byte[capacity];
            this.stream = new MemoryStream(this.buffer, false);
        }

        private MemoryStream stream;
        public Stream Stream
        {
            get { return this.stream; }
        }

        public static MidiSysExBuffer From(MidiBufferStream buffer)
        {
            int length = (int)buffer.BytesRecorded;
            var sysExBuffer = new MidiSysExBuffer(length);

            buffer.Read(sysExBuffer.buffer, 0, length);

            return sysExBuffer;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(string format)
        {
            string text = null;

            switch (format)
            {
                case "D":
                    text = ConvertToString("{0}");
                    break;

                default:
                    text = ConvertToString("{0:X2}");
                    break;
            }

            return text;
        }

        private string ConvertToString(string format)
        {
            StringBuilder text = new StringBuilder();

            foreach (byte b in this.buffer)
            {
                if (text.Length > 0)
                {
                    text.Append(ByteSeperator);
                }

                text.AppendFormat(format, b);

            }

            return text.ToString();
        }
    }
}
