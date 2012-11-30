using System;
using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents the Windows Multimedia MidiHDR structure.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiHeader
    {
        public IntPtr data;
        public uint bufferLength;
        public uint bytesRecorded;
        public uint user;
        public uint flags;
        public IntPtr next;
        public uint reserved;
        public uint offset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] reservedArray;
    }
}