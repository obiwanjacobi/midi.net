using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi
{
    public class MidiBufferStream : UnmanagedMemoryStream
    {
        // byte offsets for accessing midi header properties
        private readonly int MidiHeader_Data_Offset = 0;
        private readonly int MidiHeader_BufferLength_Offset = IntPtr.Size;
        private readonly int MidiHeader_BytesRecorded_Offset = IntPtr.Size + 2;
        private readonly int MidiHeader_Flags_Offset = IntPtr.Size + 6;
        private readonly int MidiHeader_Offset_Offset = IntPtr.Size + IntPtr.Size + 10;

        private MemoryAccessor headerAccessor;

        public unsafe MidiBufferStream(IntPtr pHeader, IntPtr pBuffer, long bufferLength, FileAccess streamAccess)
            : base((byte*)pBuffer.ToPointer(), bufferLength, bufferLength, streamAccess)
        {
            Contract.Requires<ArgumentOutOfRangeException>(bufferLength >= 0 && bufferLength <= uint.MaxValue);

            this.headerAccessor = new MemoryAccessor(pHeader, MemoryUtil.SizeOfMidiHeader);

            HeaderMemory = pHeader;
            BufferMemory = pBuffer;
            BufferLength = bufferLength;
            HeaderBufferLength = (uint)bufferLength;

            this.headerAccessor.WriteIntPtrAt(MidiHeader_Data_Offset, pBuffer);
        }

        public long BufferLength { get; private set; }

        public long BytesRecorded
        {
            get { return (long)this.headerAccessor.ReadUintAt(MidiHeader_BytesRecorded_Offset); }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>(value >= 0 && value <= uint.MaxValue);
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
        public bool IsStream
        {
            get { return ((HeaderFlags & NativeMethods.MHDR_ISSTRM) > 0); }
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        /// <remarks>Does not clear the contents only sets some properties to zero.</remarks>
        public void Clear()
        {
            // reset bytes recorded
            BytesRecorded = 0;
            HeaderBufferLength = (uint)BufferLength;
            Position = 0;
        }

        internal IntPtr HeaderMemory { get; private set; }

        internal IntPtr BufferMemory { get; private set; }

        protected internal uint HeaderBufferLength
        {
            get { return this.headerAccessor.ReadUintAt(MidiHeader_BufferLength_Offset); }
            set { this.headerAccessor.WriteUintAt(MidiHeader_BufferLength_Offset, value); }
        }

        protected internal uint HeaderFlags
        {
            get { return this.headerAccessor.ReadUintAt(MidiHeader_Flags_Offset); }
            set { this.headerAccessor.WriteUintAt(MidiHeader_Flags_Offset, value); }
        }

        protected internal uint HeaderOffset
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

    internal unsafe class MemoryAccessor
    {
        private readonly IntPtr memory;
        private readonly long length;

        public MemoryAccessor(IntPtr memory, long length)
        {
            this.memory = memory;
            this.length = length;
        }

        public void WriteIntPtrAt(int byteOffset, IntPtr value)
        {
            var size = MemoryUtil.SizeOf(typeof(IntPtr));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            *(IntPtr*)pLocation.ToPointer() = value;
        }

        public void WriteUintAt(int byteOffset, uint value)
        {
            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            *(uint*)pLocation.ToPointer() = value;
        }

        public uint ReadUintAt(int byteOffset)
        {
            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr pLocation = IntPtr.Add(this.memory, byteOffset);

            return *(uint*)pLocation.ToPointer();
        }

        private void ValidateAccess(int offset, int size)
        {
            Contract.Requires<ArgumentOutOfRangeException>(offset > 0);
            Contract.Requires<ArgumentOutOfRangeException>(size > 0);

            if (this.length < (offset + size))
            {
                var msg = String.Format(
                    "Reading {1} bytes at position {0} would cross memory boundary. Length: {2}.",
                    offset, size, this.length);
                throw new ArgumentOutOfRangeException(msg);
            }
        }
    }
}