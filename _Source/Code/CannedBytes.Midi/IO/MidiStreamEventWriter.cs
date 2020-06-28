namespace CannedBytes.Midi.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// The MidiStreamEventWriter class writes short or long midi messages
    /// into a <see cref="MidiBufferStream"/>.
    /// </summary>
    public class MidiStreamEventWriter : DisposableBase
    {
        /// <summary>
        /// Constructs a new instance on the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A stream provided by a <see cref="MidiOutStreamPort"/>. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public MidiStreamEventWriter(MidiBufferStream stream)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Check.IfArgumentNull(stream, "stream");
            if (!stream.CanWrite)
            {
                throw new ArgumentException(
                    Properties.Resources.MidiStreamWriter_StreamNotWritable, "stream");
            }

            this.BaseStream = stream;
            this.InnerWriter = new BinaryWriter(stream);
        }

        /// <summary>
        /// Object Invariant Contract.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(this.BaseStream != null);
            Contract.Invariant(this.InnerWriter != null);
        }

        /// <summary>
        /// Gets a binary writer derived classes can use to write data to the stream.
        /// </summary>
        protected BinaryWriter InnerWriter { get; private set; }

        /// <summary>
        /// Gets the stream this writer is acting on.
        /// </summary>
        public MidiBufferStream BaseStream { get; private set; }

        /// <summary>
        /// Returns true if the stream has room to write one short midi message.
        /// </summary>
        /// <returns>Returns false if there is no more room.</returns>
        public bool CanWriteShort()
        {
            return this.CanWriteLong(null);
        }

        /// <summary>
        /// Checks if there is room to write the specified <paramref name="longMessage"/> into the stream.
        /// </summary>
        /// <param name="longMessage">A buffer containing the long midi message. Can be null.</param>
        /// <returns>Returns false if there is no more room.</returns>
        /// <remarks>If the <paramref name="longMessage"/> is null,
        /// the method checks the stream for a short midi message.</remarks>
        public bool CanWriteLong(byte[] longMessage)
        {
            ThrowIfDisposed();

            int size = GetMessageSize(longMessage);

            return (this.BaseStream.Position + size) < this.BaseStream.Capacity;
        }

        /// <summary>
        /// Helper to determine the size in bytes of a message.
        /// </summary>
        /// <param name="longMessage">Can be null.</param>
        /// <returns>Returns the size in bytes.</returns>
        private static int GetMessageSize(byte[] longMessage)
        {
            // size of a MidiEvent structure in the buffer
            int size = 3 * 4; // 3 integers each 4 bytes

            if (longMessage != null)
            {
                size += longMessage.Length;

                // DWORD aligned records.
                int carry = (int)(size % 4);

                size += carry;
            }

            return size;
        }

        /// <summary>
        /// Writes a short midi message to the stream.
        /// </summary>
        /// <param name="value">The short midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public void WriteShort(int value, int deltaTime)
        {
            ThrowIfDisposed();

            MidiEventData data = new MidiEventData(value);
            data.EventType = MidiEventType.ShortMessage;

            this.WriteEvent(data, deltaTime, null);
        }

        /// <summary>
        /// Writes a long midi message to the stream.
        /// </summary>
        /// <param name="longMessage">A buffer containing the long midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public void WriteLong(byte[] longMessage, int deltaTime)
        {
            Contract.Requires(longMessage != null);
            ThrowIfDisposed();
            Check.IfArgumentNull(longMessage, "longMsg");

            MidiEventData data = new MidiEventData();
            data.Length = longMessage.Length;
            data.EventType = MidiEventType.LongMessage;

            this.WriteEvent(data, deltaTime, longMessage);
        }

        /// <summary>
        /// Writes a tempo event to the stream.
        /// </summary>
        /// <param name="tempo">The new tempo in uSecs/Quarter note.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public void WriteTempo(int tempo, int deltaTime)
        {
            ThrowIfDisposed();

            MidiEventData data = new MidiEventData();
            data.Tempo = tempo;
            data.EventType = MidiEventType.ShortTempo;

            this.WriteEvent(data, deltaTime, null);
        }

        /// <summary>
        /// Inserts a marker into the stream for a callback to the client.
        /// </summary>
        /// <param name="deltaTime">A time indication of the midi event.</param>
        public void WriteCallback(int deltaTime)
        {
            ThrowIfDisposed();

            MidiEventData data = new MidiEventData();
            data.EventType = MidiEventType.ShortNopCallback;

            this.WriteEvent(data, deltaTime, null);
        }

        /// <summary>
        /// Writes a midi event to stream.
        /// </summary>
        /// <param name="midiEvent">The midi event data.</param>
        /// <param name="deltaTime">A time indication of the midi event.</param>
        /// <param name="longData">Optional long message data. Can be null.</param>
        /// <remarks>Refer to the Win32 MIDIEVNT structure for more information.</remarks>
        public void WriteEvent(int midiEvent, int deltaTime, byte[] longData)
        {
            ThrowIfDisposed();
            if (!this.CanWriteLong(longData))
            {
                throw new MidiStreamException(Properties.Resources.MidiStream_EndOfStream);
            }

            this.InnerWriter.Write(deltaTime);
            this.InnerWriter.Write(0);   // streamID
            this.InnerWriter.Write(midiEvent);

            if (longData != null)
            {
                this.InnerWriter.Write(longData, 0, longData.Length);

                // DWORD aligned records.
                long length = longData.Length;
                int rest = (int)(length % 4);

                for (int i = 0; i < rest; i++)
                {
                    this.InnerWriter.Write((byte)0);
                }
            }

            // add to bytes recorded.
            this.BaseStream.BytesRecorded += GetMessageSize(longData);
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