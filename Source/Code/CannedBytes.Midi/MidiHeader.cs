namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the Windows Multimedia MidiHDR structure.
    /// </summary>
    /// <remarks>Currently only used to calculate the size in unmanaged memory.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiHeader
    {
        /// <summary>Midi header property.</summary>
        public IntPtr Data;

        /// <summary>Midi header property.</summary>
        public uint BufferLength;

        /// <summary>Midi header property.</summary>
        public uint BytesRecorded;

        /// <summary>Midi header property.</summary>
        public uint User;

        /// <summary>Midi header property.</summary>
        public uint Flags;

        /// <summary>Midi header property.</summary>
        public IntPtr Successor;

        /// <summary>Midi header property.</summary>
        public uint Reserved;

        /// <summary>Midi header property.</summary>
        public uint Offset;

        /// <summary>Midi header property.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] ReservedArray;
    }
}