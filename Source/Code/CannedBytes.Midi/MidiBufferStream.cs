using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents a continues block of memory space that is used to
    /// transfer large pieces of midi data to and from the <see cref="MidiPort"/>s.
    /// </summary>
    /// <remarks>Although the class uses unmanaged memory pointers it does not own this memory,
    /// there for <see cref="M:Dispose"/> need not be called.</remarks>
    public sealed class MidiBufferStream : UnmanagedMemoryStream
    {
        // byte offsets for accessing midi header properties
        private readonly int MidiHeader_Data_Offset = 0;
        private readonly int MidiHeader_BufferLength_Offset = IntPtr.Size;
        private readonly int MidiHeader_BytesRecorded_Offset = IntPtr.Size + 4;
        private readonly int MidiHeader_Flags_Offset = IntPtr.Size + 8;
        private readonly int MidiHeader_Offset_Offset = IntPtr.Size + IntPtr.Size + 12;

        private MemoryAccessor headerAccessor;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="pHeader">A pointer to the unmanaged midi header memory.</param>
        /// <param name="pBuffer">A pointer to the unmanaged midi buffer memory.</param>
        /// <param name="bufferLength">The total length (in bytes) of the buffer.</param>
        /// <param name="streamAccess">The access the <see cref="Stream"/> provides to the underlying buffer.</param>
        internal unsafe MidiBufferStream(IntPtr pHeader, IntPtr pBuffer, long bufferLength, FileAccess streamAccess)
            : base((byte*)pBuffer.ToPointer(), bufferLength, bufferLength, streamAccess)
        {
            #region Method Checks

            Contract.Requires(pHeader != IntPtr.Zero);
            Contract.Requires(pBuffer != IntPtr.Zero);
            Contract.Requires(bufferLength >= 0 && bufferLength <= uint.MaxValue);
            Throw.IfArgumentNull(pHeader, "pHeader");
            Throw.IfArgumentNull(pBuffer, "pBuffer");
            Throw.IfArgumentOutOfRange(bufferLength, 0, uint.MaxValue, "bufferLength");

            #endregion Method Checks

            this.headerAccessor = new MemoryAccessor(pHeader, MemoryUtil.SizeOfMidiHeader);
            this.headerAccessor.Clear();

            HeaderMemory = pHeader;
            BufferMemory = pBuffer;
            HeaderBufferLength = (uint)bufferLength;

            // the header points to the buffer
            this.headerAccessor.WriteIntPtrAt(MidiHeader_Data_Offset, pBuffer);
        }

        /// <summary>
        /// Gets the number of bytes that have been recorded by <see cref="MidiInPort"/>.
        /// </summary>
        public long BytesRecorded
        {
            get { return (long)this.headerAccessor.ReadUintAt(MidiHeader_BytesRecorded_Offset); }
            set
            {
                //Contract.Requires<ArgumentOutOfRangeException>(value >= 0 && value <= uint.MaxValue);
                this.headerAccessor.WriteUintAt(MidiHeader_BytesRecorded_Offset, (uint)value);

                // We keep the HeaderBufferLength in sync with the recorder bytes
                // because the MidiOutPort looks at the HeaderBufferLength and
                // the MidiInPort looks at the (Header)BytesRecorded.
                HeaderBufferLength = (uint)value;
            }
        }

        /// <summary>
        /// Gets a value indicating if this buffer contains a midi (out) stream.
        /// </summary>
        public bool IsMidiStream
        {
            get { return ((HeaderFlags & NativeMethods.MHDR_ISSTRM) > 0); }
        }

        /// <summary>
        /// Clears the buffer. Call before use.
        /// </summary>
        /// <remarks>Does not clear the contents only sets some properties to zero.</remarks>
        public void Clear()
        {
            // reset bytes recorded
            BytesRecorded = 0;
            HeaderBufferLength = (uint)Capacity;
            Position = 0;
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
            get { return this.headerAccessor.ReadUintAt(MidiHeader_BufferLength_Offset); }
            set { this.headerAccessor.WriteUintAt(MidiHeader_BufferLength_Offset, value); }
        }

        /// <summary>
        /// Gets or sets the midi header flags.
        /// </summary>
        internal uint HeaderFlags
        {
            get { return this.headerAccessor.ReadUintAt(MidiHeader_Flags_Offset); }
            set { this.headerAccessor.WriteUintAt(MidiHeader_Flags_Offset, value); }
        }

        /// <summary>
        /// Gets or sets the midi header offset.
        /// </summary>
        /// <remarks>Only used by the <see cref="MidiOutStreamPort"/> for callback events.</remarks>
        internal uint HeaderOffset
        {
            get { return this.headerAccessor.ReadUintAt(MidiHeader_Offset_Offset); }
            set { this.headerAccessor.WriteUintAt(MidiHeader_Offset_Offset, value); }
        }

        /// <summary>
        /// Returns the pointer that can be passed to the midi port.
        /// </summary>
        /// <returns>Never returns IntPtr.Zero.</returns>
        public IntPtr ToIntPtr()
        {
            return HeaderMemory;
        }
    }

    /// <summary>
    /// A helper class that allows randomly accessing unmanaged memory.
    /// </summary>
    internal unsafe class MemoryAccessor
    {
        private readonly IntPtr memory;
        private readonly long length;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="memory">A pointer to the start of the memory block.</param>
        /// <param name="length">The length in bytes of the memory block.</param>
        public MemoryAccessor(IntPtr memory, long length)
        {
            this.memory = memory;
            this.length = length;
        }

        /// <summary>
        /// Writes a <paramref name="value"/> to the specified <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits)</param>
        /// <param name="value">The value to write.</param>
        public void WriteIntPtrAt(int byteOffset, IntPtr value)
        {
            var size = MemoryUtil.SizeOf(typeof(IntPtr));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            *(IntPtr*)pLocation.ToPointer() = value;
        }

        /// <summary>
        /// Writes a <paramref name="value"/> to the specified <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits)</param>
        /// <param name="value">The value to write.</param>
        public void WriteUintAt(int byteOffset, uint value)
        {
            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            *(uint*)pLocation.ToPointer() = value;
        }

        /// <summary>
        /// Reads a unsigned integer from the memory block at the <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits)</param>
        /// <returns>Returns the value.</returns>
        public uint ReadUintAt(int byteOffset)
        {
            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            return *(uint*)pLocation.ToPointer();
        }

        /// <summary>
        /// Throws an exception when the memory block bounds are about to be violated.
        /// </summary>
        /// <param name="offset">Must be greater than or equal to zero.</param>
        /// <param name="size">Must be greater than zero.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when there is a problem
        /// with <paramref name="offset"/> or <paramref name="size"/>.</exception>
        private void ValidateAccess(int offset, int size)
        {
            //Contract.Requires<ArgumentOutOfRangeException>(offset >= 0);
            //Contract.Requires<ArgumentOutOfRangeException>(size > 0);

            if (this.length < (offset + size))
            {
                var msg = String.Format(
                    "Reading {1} bytes at position {0} would cross memory boundary. Length: {2}.",
                    offset, size, this.length);
                throw new ArgumentOutOfRangeException(msg);
            }
        }

        public unsafe void Clear()
        {
            byte* pMem = (byte*)this.memory.ToPointer();

            for (int i = 0; i < this.length; i++)
            {
                *pMem = 0;
                pMem++;
            }
        }
    }
}