using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CannedBytes.Midi.MidiThruTestApp
{
    /// <summary>
    /// Represents the Windows Multimedia MIDIINCAPS structure.
    /// </summary>
    internal struct MidiInCaps
    {
        public ushort mid;
        public ushort pid;
        public uint driverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
        public uint support;
    }

    /// <summary>
    /// Represents the Windows Multimedia MIDIOUTCAPS structure.
    /// </summary>
    internal struct MidiOutCaps
    {
        public ushort mid;
        public ushort pid;
        public uint driverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
        public ushort technology;
        public ushort voices;
        public ushort notes;
        public ushort channelMask;
        public uint support;
    }
}
