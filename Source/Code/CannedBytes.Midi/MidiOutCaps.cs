namespace CannedBytes.Midi
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the Windows Multimedia MIDIOUTCAPS structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiOutCaps
    {
        /// <summary>Midi out caps property.</summary>
        public ushort Mid;

        /// <summary>Midi out caps property.</summary>
        public ushort Pid;

        /// <summary>Midi out caps property.</summary>
        public uint DriverVersion;

        /// <summary>Midi out caps property.</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        /// <summary>Midi out caps property.</summary>
        public ushort Technology;

        /// <summary>Midi out caps property.</summary>
        public ushort Voices;

        /// <summary>Midi out caps property.</summary>
        public ushort Notes;

        /// <summary>Midi out caps property.</summary>
        public ushort ChannelMask;

        /// <summary>Midi out caps property.</summary>
        public uint Support;
    }
}