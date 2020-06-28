namespace CannedBytes.Midi.IO
{
    using CannedBytes.Midi.Message;
    using System;
    using System.IO;

    /// <summary>
    /// Writes a midi track to a stream.
    /// </summary>
    public class MidiFileStreamWriter : DisposableBase
    {
        /// <summary>An internal binary writer.</summary>
        private readonly BinaryWriter _writer;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        public MidiFileStreamWriter(Stream stream)
        {
            Check.IfArgumentNull(stream, nameof(stream));
            if (!stream.CanWrite)
            {
                throw new ArgumentException("The stream does not support writing.", nameof(stream));
            }

            BaseStream = stream;
            _writer = new BinaryWriter(stream);
        }

        /// <summary>
        /// Gets the stream that is written to.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Previous status of a midi event used for running status.</summary>
        private int _lastStatus;

        /// <summary>
        /// Writes a midi (short) event to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="data">The midi short event data.</param>
        public void WriteMidiEvent(long deltaTime, int data)
        {
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, nameof(deltaTime));

            var midiData = new MidiData(data);
            var status = midiData.Status;

            WriteVariableLength((uint)deltaTime);

            // running status
            if (status != _lastStatus)
            {
                _lastStatus = status;

                _writer.Write(status);
            }

            if (midiData.HasParameter1)
            {
                _writer.Write(midiData.Parameter1);

                if (midiData.HasParameter2)
                {
                    _writer.Write(midiData.Parameter2);
                }
            }
        }

        /// <summary>
        /// Writes a meta event to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="type">The type of meta event.</param>
        /// <param name="data">The data for the meta event. Must not be null.</param>
        public void WriteMetaEvent(long deltaTime, MidiMetaType type, byte[] data)
        {
            Check.IfArgumentNull(data, nameof(data));
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, nameof(deltaTime));
            _lastStatus = 0;

            WriteVariableLength((uint)deltaTime);

            // meta data marker
            _writer.Write((byte)0xFF);

            // meta type
            _writer.Write((byte)type);

            // length of data
            WriteVariableLength((uint)data.Length);

            // meta data
            _writer.Write(data);
        }

        /// <summary>
        /// Writes a system exclusive message to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="data">The message data. It is assumed that NO sysex markers are present in the data. Must not be null.</param>
        /// <param name="isContinuation">An indication if this message is a continuation of a previous sysex message.</param>
        public void WriteSysExEvent(long deltaTime, byte[] data, bool isContinuation)
        {
            Check.IfArgumentNull(data, nameof(data));
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, nameof(deltaTime));
            _lastStatus = 0;

            WriteVariableLength((uint)deltaTime);

            uint length = (uint)data.Length;

            if (isContinuation)
            {
                length++;
                _writer.Write(0xF7);
            }
            else
            {
                length += 2;
                _writer.Write(0xF0);
            }

            // length of data
            WriteVariableLength(length);

            // sysex data
            _writer.Write(data);

            if (!isContinuation)
            {
                _writer.Write(0xF7);
            }
        }

        /// <summary>
        /// Writes a variable length <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        private void WriteVariableLength(uint value)
        {
            uint buffer = value & 0x7F;

            // build buffer with bytes
            while ((value >>= 7) > 0)
            {
                buffer <<= 8;
                buffer |= (value & 0x7F) | 0x80;
            }

            while (true)
            {
                _writer.Write((byte)buffer);

                if ((buffer & 0x80) > 0)
                {
                    buffer >>= 8;
                }
                else
                {
                    break;
                }
            }
        }

        /// <inheritdocs/>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed &&
                disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                BaseStream.Dispose();
            }
        }
    }
}