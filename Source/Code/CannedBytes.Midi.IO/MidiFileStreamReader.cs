using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi.IO
{
    /// <summary>
    /// Reads the midi file track information and provides this info as structured data.
    /// </summary>
    public class MidiFileStreamReader : DisposableBase
    {
        /// <summary>
        /// Constructs a new instance on the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        public MidiFileStreamReader(Stream stream)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead, "The stream does not support reading.");
            Throw.IfArgumentNull(stream, "stream");
            if (!stream.CanRead)
            {
                throw new ArgumentException("The stream does not support reading.", "stream");
            }

            BaseStream = stream;
        }

        /// <summary>
        /// Gets the file stream that is read.
        /// </summary>
        public Stream BaseStream { get; protected set; }

        /// <summary>
        /// Reads the next event in the midi file.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        public virtual bool ReadNextEvent()
        {
            // end of stream
            if (BaseStream.Position >= BaseStream.Length) return false;

            bool success = ReadDeltaTime();
            byte status = SafeReadByte();

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

        /// <summary>
        /// Reads the delta-time for the midi event.
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        /// <remarks>Sets the <see cref="P:DeltaTime"/> and <see cref="AbsoluteTime"/> properties.</remarks>
        private bool ReadDeltaTime()
        {
            DeltaTime = ReadVariableLength();
            AbsoluteTime += DeltaTime;

            return true;
        }

        /// <summary>
        /// Reads the event based on the <paramref name="status"/>.
        /// </summary>
        /// <param name="status">First byte of the tot event.</param>
        /// <returns>Returns true when successful.</returns>
        private bool ReadEvent(byte status)
        {
            var data = new MidiData();

            if ((status & 0x80) == 0)
            {
                // copy running status from last event
                data.Status = MidiData.GetStatus(MidiEvent);
                data.Parameter1 = status;
            }
            else
            {
                data.Status = status;

                if (data.HasParameter1)
                {
                    data.Parameter1 = SafeReadByte();
                }
            }

            if (data.HasParameter2)
            {
                data.Parameter2 = SafeReadByte();
            }

            MidiEvent = data;

            return true;
        }

        /// <summary>
        /// Reads the sysex event.
        /// </summary>
        /// <param name="status">The first byte of the sysex message.</param>
        /// <returns>Returns true when successful.</returns>
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

        /// <summary>
        /// Reads a meta event.
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        private bool ReadMetaEvent()
        {
            MetaEvent = SafeReadByte();
            uint length = ReadVariableLength();

            Data = new byte[length];

            return (SafeReadData(0) == length);
        }

        /// <summary>
        /// Gets the type of midi event that was read after a call to <see cref="M:ReadNextEvent"/>.
        /// </summary>
        public MidiFileEventType EventType { get; protected set; }

        /// <summary>
        /// Gets the delta-time for the current midi event.
        /// </summary>
        public long DeltaTime { get; protected set; }

        /// <summary>
        /// Gets the absolute-time for the current midi event.
        /// </summary>
        public long AbsoluteTime { get; protected set; }

        /// <summary>
        /// Gets the midi event data.
        /// </summary>
        public int MidiEvent { get; protected set; }

        /// <summary>
        /// Gets the meta event type.
        /// </summary>
        public byte MetaEvent { get; protected set; }

        /// <summary>
        /// Gets the data for a SysEx or meta event.
        /// </summary>
        public byte[] Data { get; protected set; }

        /// <summary>
        /// Used to read bytes into <see cref="P:Data"/>.
        /// </summary>
        /// <param name="index">The index into <see cref="Data"/> where to start.</param>
        /// <returns>Returns the number of bytes read.</returns>
        private uint SafeReadData(int index)
        {
            uint length = (uint)(Data.Length - index);
            return SafeRead(Data, index, length);
        }

        // TODO: uint index
        /// <summary>
        /// Reads a number of bytes <paramref name="length"/> into the <paramref name="buffer"/> starting at <paramref name="index"/>.
        /// </summary>
        /// <param name="buffer">Must not be null and large enough for <paramref name="length"/> bytes.</param>
        /// <param name="index">Index into the <paramref name="buffer"/> where to start.</param>
        /// <param name="length">The total length to write into the <paramref name="buffer"/>.</param>
        /// <returns>Returns the number of bytes read.</returns>
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

        /// <summary>
        /// Reads a single byte and throws an <see cref="EndOfStreamException"/> if the stream is at the end.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        private byte SafeReadByte()
        {
            int value = BaseStream.ReadByte();

            if (value == -1)
            {
                throw new EndOfStreamException();
            }

            return (byte)value;
        }

        /// <summary>
        /// Reads a variable length value.
        /// </summary>
        /// <returns>Returns the value read.</returns>
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

        /// <inheritdocs/>
        protected override void Dispose(bool disposing)
        {
            BaseStream.Dispose();

            base.Dispose(disposing);
        }
    }
}