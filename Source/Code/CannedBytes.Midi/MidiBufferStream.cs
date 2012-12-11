namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Represents a continues block of memory space that is used to
    /// transfer large pieces of midi data to and from the <see cref="MidiPort"/>s.
    /// </summary>
    /// <remarks>Although the class uses unmanaged memory pointers it does not own this memory,
    /// there for <see cref="M:Dispose"/> need not be called.</remarks>
    public sealed class MidiBufferStream : UnmanagedMemoryStream
    {
        /// <summary>Byte offset into header memory for the Data property.</summary>
        private readonly int MidiHeaderDataOffset = 0;

        /// <summary>Byte offset into header memory for the BufferLength property.</summary>
        private readonly int MidiHeaderBufferLengthOffset = IntPtr.Size;

        /// <summary>Byte offset into header memory for the BytesRecorded property.</summary>
        private readonly int MidiHeaderBytesRecordedOffset = IntPtr.Size + 4;

        /// <summary>Byte offset into header memory for the Flags property.</summary>
        private readonly int MidiHeaderFlagsOffset = IntPtr.Size + 8;

        /// <summary>Byte offset into header memory for the Offset property.</summary>
        private readonly int MidiHeaderOffsetOffset = IntPtr.Size + IntPtr.Size + 12;

        /// <summary>Accessor for writing an reading the unmanaged header memory.</summary>
        private readonly UnmanagedMemoryAccessor headerAccessor;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="headerMem">A pointer to the unmanaged midi header memory.</param>
        /// <param name="bufferMem">A pointer to the unmanaged midi buffer memory.</param>
        /// <param name="bufferLength">The total length (in bytes) of the buffer.</param>
        /// <param name="streamAccess">The access the <see cref="Stream"/> provides to the underlying buffer.</param>
        internal unsafe MidiBufferStream(IntPtr headerMem, IntPtr bufferMem, long bufferLength, FileAccess streamAccess)
            : base((byte*)bufferMem.ToPointer(), bufferLength, bufferLength, streamAccess)
        {
            Contract.Requires(headerMem != IntPtr.Zero);
            Contract.Requires(bufferMem != IntPtr.Zero);
            Contract.Requires(bufferLength >= 0 && bufferLength <= uint.MaxValue);
            Throw.IfArgumentNull(headerMem, "headerMem");
            Throw.IfArgumentNull(bufferMem, "bufferMem");
            Throw.IfArgumentOutOfRange(bufferLength, 0, uint.MaxValue, "bufferLength");

            this.headerAccessor = new UnmanagedMemoryAccessor(headerMem, MemoryUtil.SizeOfMidiHeader);
            this.headerAccessor.Clear();

            this.HeaderMemory = headerMem;
            this.BufferMemory = bufferMem;
            this.HeaderBufferLength = (uint)bufferLength;

            // the header points to the buffer
            this.headerAccessor.WriteIntPtrAt(this.MidiHeaderDataOffset, bufferMem);
        }

        /// <summary>
        /// Gets the number of bytes that have been recorded by <see cref="MidiInPort"/>.
        /// </summary>
        public long BytesRecorded
        {
            get
            {
                return (long)this.headerAccessor.ReadUintAt(this.MidiHeaderBytesRecordedOffset);
            }

            set
            {
                Contract.Requires(value >= 0 && value <= uint.MaxValue);
                Throw.IfArgumentOutOfRange(value, 0, uint.MaxValue, "BytesRecorded");

                this.headerAccessor.WriteUintAt(this.MidiHeaderBytesRecordedOffset, (uint)value);

                // We keep the HeaderBufferLength in sync with the recorder bytes
                // because the MidiOutPort looks at the HeaderBufferLength and
                // the MidiInPort looks at the (Header)BytesRecorded.
                this.HeaderBufferLength = (uint)value;
            }
        }

        /// <summary>
        /// Gets a value indicating if this buffer contains a midi (out) stream.
        /// </summary>
        public bool IsMidiStream
        {
            get { return (this.HeaderFlags & NativeMethods.MHDR_ISSTRM) > 0; }
        }

        /// <summary>
        /// Clears the buffer. Call before use.
        /// </summary>
        /// <remarks>Does not clear the contents only sets some properties to zero.</remarks>
        public void Clear()
        {
            // reset bytes recorded
            this.BytesRecorded = 0;
            this.HeaderBufferLength = (uint)Capacity;
            this.Position = 0;
        }

        /// <summary>
        /// Gets the pointer to the midi header.
        /// </summary>
        internal IntPtr HeaderMemory { get; private set; }

        /// <summary>
        /// Gets the pointer to the buffer.
        /// </summary>
        internal IntPtr BufferMemory { get; private set; }

        /// <summary>
        /// Gets or sets the midi header buffer length value.
        /// </summary>
        /// <remarks>Note that the <see cref="MidiOutPort"/> and the <see cref="MidiOutStreamPort"/>
        /// use this value to determine how many bytes to send.</remarks>
        internal uint HeaderBufferLength
        {
            get { return this.headerAccessor.ReadUintAt(this.MidiHeaderBufferLengthOffset); }
            set { this.headerAccessor.WriteUintAt(this.MidiHeaderBufferLengthOffset, value); }
        }

        /// <summary>
        /// Gets or sets the midi header flags.
        /// </summary>
        internal uint HeaderFlags
        {
            get { return this.headerAccessor.ReadUintAt(this.MidiHeaderFlagsOffset); }
            set { this.headerAccessor.WriteUintAt(this.MidiHeaderFlagsOffset, value); }
        }

        /// <summary>
        /// Gets or sets the midi header offset.
        /// </summary>
        /// <remarks>Only used by the <see cref="MidiOutStreamPort"/> for callback events.</remarks>
        internal uint HeaderOffset
        {
            get { return this.headerAccessor.ReadUintAt(this.MidiHeaderOffsetOffset); }
            set { this.headerAccessor.WriteUintAt(this.MidiHeaderOffsetOffset, value); }
        }

        /// <summary>
        /// Returns the pointer that can be passed to the midi port.
        /// </summary>
        /// <returns>Never returns IntPtr.Zero.</returns>
        public IntPtr ToIntPtr()
        {
            return this.HeaderMemory;
        }
    }
}