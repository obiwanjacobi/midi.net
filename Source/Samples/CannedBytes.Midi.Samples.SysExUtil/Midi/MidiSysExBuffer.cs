using System.IO;
using System.Text;

namespace CannedBytes.Midi.Samples.SysExUtil.Midi
{
    internal sealed class MidiSysExBuffer
    {
        public const char ByteSeperator = ' ';

        private readonly byte[] _buffer;

        public MidiSysExBuffer(int capacity)
        {
            _buffer = new byte[capacity];
            _stream = new MemoryStream(_buffer, true);
        }

        private readonly MemoryStream _stream;
        public Stream Stream
        {
            get { return _stream; }
        }

        public static MidiSysExBuffer From(MidiBufferStream buffer)
        {
            int length = (int)buffer.BytesRecorded;
            var sysExBuffer = new MidiSysExBuffer(length);

            buffer.Position = 0;

            buffer.Read(sysExBuffer._buffer, 0, length);

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

            foreach (byte b in _buffer)
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
