namespace CannedBytes.Midi.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using CannedBytes.Midi.Message;

    /// <summary>
    /// Writes a midi track to a stream.
    /// </summary>
    public class MidiFileStreamWriter : DisposableBase
    {
        /// <summary>An internal binary writer.</summary>
        private BinaryWriter writer;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
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

        /// <summary>
        /// Writes a midi (short) event to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="data">The midi short event data.</param>
        public void WriteMidiEvent(long deltaTime, int data)
        {
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");

            var midiData = new MidiData(data);
            var status = midiData.Status;

            this.WriteVariableLength((uint)deltaTime);

            // running status
            if (status != this.lastStatus)
            {
                this.lastStatus = status;

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

        /// <summary>
        /// Writes a meta event to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="type">The type of meta event.</param>
        /// <param name="data">The data for the meta event. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Check is not recognized.")]
        public void WriteMetaEvent(long deltaTime, MidiMetaType type, byte[] data)
        {
            Check.IfArgumentNull(data, "data");
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");
            this.lastStatus = 0;

            this.WriteVariableLength((uint)deltaTime);

            // meta data marker
            this.writer.Write((byte)0xFF);

            // meta type
            this.writer.Write((byte)type);

            // length of data
            this.WriteVariableLength((uint)data.Length);

            // meta data
            this.writer.Write(data);
        }

        /// <summary>
        /// Writes a system exclusive message to the stream.
        /// </summary>
        /// <param name="deltaTime">Must be greater or equal to zero.</param>
        /// <param name="data">The message data. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Check is not recognized.")]
        public void WriteSysExEvent(long deltaTime, byte[] data)
        {
            Check.IfArgumentNull(data, "data");
            Check.IfArgumentOutOfRange(deltaTime, 0, uint.MaxValue, "deltaTime");
            this.lastStatus = 0;

            this.WriteVariableLength((uint)deltaTime);

            // length of data
            this.WriteVariableLength((uint)data.Length);

            // meta data
            this.writer.Write(data);
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