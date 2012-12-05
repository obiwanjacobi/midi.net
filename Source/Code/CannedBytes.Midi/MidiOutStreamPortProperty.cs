using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiOutStreamPortProperty
    {
        public MidiOutStreamPortProperty(uint value)
        {
            cbStruct = (uint)MemoryUtil.SizeOfMidiStreamOutPortProperty;
            propertyValue = value;
        }

        public uint cbStruct;
        public uint propertyValue;
    }
}