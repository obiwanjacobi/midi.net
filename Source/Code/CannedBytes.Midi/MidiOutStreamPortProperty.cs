namespace CannedBytes.Midi
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The unmanaged structure for Stream Port properties.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiOutStreamPortProperty
    {
        /// <summary>
        /// Constructs a new instance with a <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Can be zero.</param>
        public MidiOutStreamPortProperty(uint value)
        {
            this.StructSize = (uint)MemoryUtil.SizeOfMidiStreamOutPortProperty;
            this.PropertyValue = value;
        }

        /// <summary>Unmanaged structure property.</summary>
        public uint StructSize;

        /// <summary>Unmanaged structure property.</summary>
        public uint PropertyValue;
    }
}