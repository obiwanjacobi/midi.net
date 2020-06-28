namespace CannedBytes.Midi
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Internal time structure passed to <see cref="NativeMethods"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct MmTime
    {
        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(0)]
        public uint Type;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public uint MilliSeconds;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public uint Sample;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public uint ByteCount;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public uint Ticks;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public uint MidiSongPtrPos;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(4)]
        public byte SmpteHour;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(5)]
        public byte SmpteMin;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(6)]
        public byte SmpteSec;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(7)]
        public byte SmpteFrame;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(8)]
        public byte SmpteFps;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(9)]
        public byte SmpteDummy;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(10)]
        public byte SmptePad0;

        /// <summary>Unmanaged MmTime property.</summary>
        [FieldOffset(11)]
        public byte SmptePad1;
    }
}