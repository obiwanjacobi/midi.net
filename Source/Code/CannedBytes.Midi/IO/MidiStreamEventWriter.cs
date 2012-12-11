namespace CannedBytes.Midi.IO
{
    using System;
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
        public MidiStreamEventWriter(MidiBufferStream stream)
        {
            Contract.Requires(stream != null);
            Contract.Requires(stream.CanRead);
            Throw.IfArgumentNull(stream, "stream");
            if (!stream.CanWrite)
            {
                throw new ArgumentException(
                    Properties.Resources.MidiStreamWriter_StreamNotWritable, "stream");
            }

            this.BaseStream = stream;
            this.InnerWritter = new BinaryWriter(stream);
        }

        /// <summary>
        /// Object Invariant Contract.
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(this.BaseStream != null);
            Contract.Invariant(this.InnerWritter != null);
        }

        /// <summary>
        /// Gets a binary writer derived classes can use to write data to the stream.
        /// </summary>
        protected BinaryWriter InnerWritter { get; private set; }

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
        /// Checks if there is room to write the specified <paramref name="longMsg"/> into the stream.
        /// </summary>
        /// <param name="longMsg">A buffer containing the long midi message. Can be null.</param>
        /// <returns>Returns false if there is no more room.</returns>
        /// <remarks>If the <paramref name="longMsg"/> is null,
        /// the method checks the stream for a short midi message.</remarks>
        public bool CanWriteLong(byte[] longMsg)
        {
            ThrowIfDisposed();

            int size = GetMessageSize(longMsg);

            return (this.BaseStream.Position + size) < this.BaseStream.Capacity;
        }

        /// <summary>
        /// Helper to determine the size in bytes of a message.
        /// </summary>
        /// <param name="longMsg">Can be null.</param>
        /// <returns>Returns the size in bytes.</returns>
        private static int GetMessageSize(byte[] longMsg)
        {
            // size of a MidiEvent structure in the buffer
            int size = 3 * 4; // 3 integers each 4 bytes

            if (longMsg != null)
            {
                size += longMsg.Length;

                // DWORD aligned records.
                int rest = (int)(size % 4);

                size += rest;
            }

            return size;
        }

        /// <summary>
        /// Writes a short midi message to the stream.
        /// </summary>
        /// <param name="shortMsg">The short midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public void WriteShort(int shortMsg, int deltaTime)
        {
            ThrowIfDisposed();

            MidiEventData data = new MidiEventData(shortMsg);
            data.EventType = MidiEventType.ShortMessage;

            this.WriteEvent(data, deltaTime, null);
        }

        /// <summary>
        /// Writes a long midi message to the stream.
        /// </summary>
        /// <param name="longMsg">A buffer containing the long midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public void WriteLong(byte[] longMsg, int deltaTime)
        {
            Contract.Requires(longMsg != null);
            ThrowIfDisposed();
            Throw.IfArgumentNull(longMsg, "longMsg");

            MidiEventData data = new MidiEventData();
            data.Length = longMsg.Length;
            data.EventType = MidiEventType.LongMessage;

            this.WriteEvent(data, deltaTime, longMsg);
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

            this.InnerWritter.Write(deltaTime);
            this.InnerWritter.Write(0);   // streamID
            this.InnerWritter.Write(midiEvent);

            if (longData != null)
            {
                this.InnerWritter.Write(longData, 0, longData.Length);

                // DWORD aligned records.
                long length = longData.Length;
                int rest = (int)(length % 4);

                for (int i = 0; i < rest; i++)
                {
                    this.InnerWritter.Write((byte)0);
                }
            }

            // add to bytes recorded.
            this.BaseStream.BytesRecorded += GetMessageSize(longData);
        }

        /// <inheritdocs/>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.BaseStream.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}