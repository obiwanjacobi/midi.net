namespace CannedBytes.Midi.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using CannedBytes.Midi.Message;

    class MidiFileStreamWriter : DisposableBase
    {
        private BinaryWriter writer;

        public MidiFileStreamWriter(Stream stream)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanWrite, "The stream does not support writing.");
            Check.IfArgumentNull(stream, "stream");
            if (!stream.CanWrite)
            {
                throw new ArgumentException("The stream does not support writing.", "stream");
            }

            this.BaseStream = stream;
            this.writer = new BinaryWriter(stream);
        }

        /// <summary>
        /// Gets the stream that is written to.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Previous status of a midi event used for running status.</summary>
        private int lastStatus;

        public void WriteMidiEvent(long deltaTime, int data)
        {
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");

            var midiData = new MidiData(data);
            var status = midiData.Status;

            WriteVariableLength((uint)deltaTime);

            // running status
            if (status != this.lastStatus)
            {
                lastStatus = status;

                this.writer.Write((byte)status);
            }

            if (midiData.HasParameter1)
            {
                this.writer.Write(midiData.Parameter1);

                if (midiData.HasParameter2)
                {
                    this.writer.Write(midiData.Parameter2);
                }
            }
        }

        public void WriteMetaEvent(long deltaTime, MidiMetaType type, byte[] data)
        {
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");
            lastStatus = 0;

            WriteVariableLength((uint)deltaTime);
            // meta data marker
            this.writer.Write((byte)0xFF);
            // meta type
            this.writer.Write((byte)type);
            // length of data
            this.WriteVariableLength((uint)data.Length);
            // meta data
            this.writer.Write(data);
        }

        public void WriteSysExEvent(long deltaTime, byte[] data)
        {
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");
            lastStatus = 0;

            WriteVariableLength((uint)deltaTime);
            // length of data
            this.WriteVariableLength((uint)data.Length);
            // meta data
            this.writer.Write(data);
        }

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
                this.writer.Write((byte)buffer);

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
            if (!IsDisposed)
            {
                if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
                {
                    this.BaseStream.Dispose();
                }
            }
        }
    }
}