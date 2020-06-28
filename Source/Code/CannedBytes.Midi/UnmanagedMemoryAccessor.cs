namespace CannedBytes.Midi
{
    using System;
    using System.Globalization;

    /// <summary>
    /// A helper class that allows randomly accessing unmanaged memory.
    /// </summary>
    internal unsafe class UnmanagedMemoryAccessor
    {
        /// <summary>A pointer to the unmanaged memory.</summary>
        private readonly IntPtr _memory;

        /// <summary>The length in bytes of the unmanaged memory.</summary>
        private readonly long _length;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="memory">A pointer to the start of the memory block.</param>
        /// <param name="length">The length in bytes of the memory block.</param>
        public UnmanagedMemoryAccessor(IntPtr memory, long length)
        {
            Check.IfArgumentOutOfRange(length, 1, long.MaxValue, nameof(length));
            if (memory == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(memory));
            }

            _memory = memory;
            _length = length;
        }

        /// <summary>
        /// Writes a <paramref name="value"/> to the specified <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits).</param>
        /// <param name="value">The value to write.</param>
        public void WriteIntPtrAt(int byteOffset, IntPtr value)
        {
            Check.IfArgumentOutOfRange(byteOffset, 0, int.MaxValue, nameof(byteOffset));

            var size = MemoryUtil.SizeOf(typeof(IntPtr));
            ValidateAccess(byteOffset, size);

            IntPtr location = IntPtr.Add(_memory, byteOffset);

            *(IntPtr*)location.ToPointer() = value;
        }

        /// <summary>
        /// Writes a <paramref name="value"/> to the specified <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits).</param>
        /// <param name="value">The value to write.</param>
        public void WriteUintAt(int byteOffset, uint value)
        {
            Check.IfArgumentOutOfRange(byteOffset, 0, int.MaxValue, nameof(byteOffset));

            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr location = IntPtr.Add(_memory, byteOffset);

            *(uint*)location.ToPointer() = value;
        }

        /// <summary>
        /// Reads a unsigned integer from the memory block at the <paramref name="byteOffset"/>.
        /// </summary>
        /// <param name="byteOffset">An offset from the start of the memory block where the writing should start.
        /// Make sure the <paramref name="byteOffset"/> is properly aligned (usually 16-bits).</param>
        /// <returns>Returns the value.</returns>
        public uint ReadUintAt(int byteOffset)
        {
            Check.IfArgumentOutOfRange(byteOffset, 0, int.MaxValue, nameof(byteOffset));

            var size = MemoryUtil.SizeOf(typeof(uint));
            ValidateAccess(byteOffset, size);

            IntPtr location = IntPtr.Add(_memory, byteOffset);

            return *(uint*)location.ToPointer();
        }

        /// <summary>
        /// Throws an exception when the memory block bounds are about to be violated.
        /// </summary>
        /// <param name="byteOffset">Must be greater than or equal to zero.</param>
        /// <param name="size">Must be greater than zero.</param>
        private void ValidateAccess(int byteOffset, int size)
        {
            Check.IfArgumentOutOfRange(byteOffset, 0, int.MaxValue, nameof(byteOffset));
            Check.IfArgumentOutOfRange(size, 1, int.MaxValue, nameof(size));

            if (_length < (byteOffset + size))
            {
                var msg = String.Format(
                          CultureInfo.InvariantCulture,
                          "Reading {1} bytes at position {0} would cross memory boundary. Length: {2}.",
                          byteOffset,
                          size,
                          _length);

                throw new ArgumentOutOfRangeException(msg);
            }
        }

        /// <summary>
        /// Zero-outs the memory block.
        /// </summary>
        public unsafe void Clear()
        {
            byte* mem = (byte*)_memory.ToPointer();

            for (int i = 0; i < _length; i++)
            {
                *mem = 0;
                mem++;
            }
        }
    }
}