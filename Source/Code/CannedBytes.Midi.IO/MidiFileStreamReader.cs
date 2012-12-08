using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi.IO
{
    public class MidiFileStreamReader
    {
        public MidiFileStreamReader(Stream stream)
        {
            Contract.Requires<ArgumentNullException>(stream != null);
            Contract.Requires<ArgumentException>(stream.CanRead, "The stream does not support reading.");

            BaseStream = stream;
        }

        public Stream BaseStream { get; protected set; }

        public virtual bool ReadNextEvent()
        {
            bool success = ReadDeltaTime();

            // don't use SafeRead - we don't want exceptions here.
            int status = BaseStream.ReadByte();
            if (status == -1) return false;

            if (status == 0xFF)
            {
                // Meta Event
                EventType = MidiFileEventType.Meta;
                success = ReadMetaEvent();
            }
            else if (status == 0xF7)
            {
                // SysEx continuation
                EventType = MidiFileEventType.SysExCont;
                success = ReadSysEx((byte)status);
            }
            else if (status == 0xF0)
            {
                // SysEx
                EventType = MidiFileEventType.SysEx;
                success = ReadSysEx((byte)status);
            }
            else //if (status != 0) // with running status 'status' can be zero.
            {
                // Midi Event
                EventType = MidiFileEventType.Event;
                success = ReadEvent((byte)status);
            }

            return ((BaseStream.Length > BaseStream.Position) && success);
        }

        private bool ReadDeltaTime()
        {
            DeltaTime = ReadVariableLength();
            return true;
        }

        private bool ReadEvent(byte status)
        {
            var data = new MidiData();

            if ((status & 0x80) == 0)
            {
                // copy running status from last event
                data.Status = MidiData.GetStatus(MidiEvent);
                data.Param1 = status;
            }
            else
            {
                data.Status = status;

                if (data.HasParam1)
                {
                    data.Param1 = SafeReadByte();
                }
            }

            if (data.HasParam2)
            {
                data.Param2 = SafeReadByte();
            }

            MidiEvent = data;

            return true;
        }

        private bool ReadSysEx(byte status)
        {
            uint length = ReadVariableLength();

            if (EventType == MidiFileEventType.SysExCont)
            {
                Data = new byte[length];

                return (SafeReadData(0) == length);
            }
            else
            {
                Data = new byte[length + 1];
                Data[0] = status;

                return (SafeReadData(1) == length);
            }
        }

        private bool ReadMetaEvent()
        {
            MetaEvent = SafeReadByte();
            uint length = ReadVariableLength();

            Data = new byte[length];

            return (SafeReadData(0) == length);
        }

        public MidiFileEventType EventType { get; protected set; }

        public long DeltaTime { get; protected set; }

        public int MidiEvent { get; protected set; }

        public byte MetaEvent { get; protected set; }

        public byte[] Data { get; protected set; }

        private uint SafeReadData(int index)
        {
            uint length = (uint)(Data.Length - index);
            return SafeRead(Data, index, length);
        }

        // TODO: uint index
        private uint SafeRead(byte[] buffer, int index, uint length)
        {
            int count = 0;
            uint bytesRead = 0;

            do
            {
                if (length > int.MaxValue)
                {
                    count = int.MaxValue;
                    length -= int.MaxValue;
                }
                else
                {
                    count = (int)length;
                    length = 0; // signal all bytes read
                }

                var actual = BaseStream.Read(buffer, index, count);
                bytesRead += (uint)actual;

                index += count;
            }
            while (length > int.MaxValue);

            return bytesRead;
        }

        private byte SafeReadByte()
        {
            int value = BaseStream.ReadByte();

            if (value == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)value;
        }

        private uint ReadVariableLength()
        {
            uint result = SafeReadByte();

            if ((result & 0x80) == 0x80)
            {
                // clear off bit7
                result &= 0x7F;

                uint value = 0;

                do
                {
                    value = SafeReadByte();

                    result <<= 7;
                    result |= value & 0x7F;
                }
                while ((value & 0x80) == 0x80);
            }

            return result;
        }
    }
}