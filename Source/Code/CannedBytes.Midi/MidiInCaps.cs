namespace CannedBytes.Midi
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents the Windows Multimedia MIDIINCAPS structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiInCaps
    {
        /// <summary>Midi in caps property.</summary>
        public ushort Mid;

        /// <summary>Midi in caps property.</summary>
        public ushort Pid;

        /// <summary>Midi in caps property.</summary>
        public uint DriverVersion;

        /// <summary>Midi in caps property.</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string Name;

        /// <summary>Midi in caps property.</summary>
        public uint Support;
    }
}