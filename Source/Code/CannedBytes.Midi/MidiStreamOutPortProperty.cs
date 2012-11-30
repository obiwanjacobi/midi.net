using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MidiStreamOutPortProperty
    {
        public MidiStreamOutPortProperty(uint value)
        {
            cbStruct = (uint)MemoryUtil.SizeOfMidiStreamOutPortProperty;
            propertyValue = value;
        }

        public uint cbStruct;
        public uint propertyValue;
    }
}