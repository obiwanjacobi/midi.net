using System;
using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Helper class for unmanaged memory operations.
    /// </summary>
    internal static class MemoryUtil
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static int SizeOf(Type type)
        {
            return Marshal.SizeOf(type);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void Free(IntPtr memory)
        {
            Marshal.FreeHGlobal(memory);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static IntPtr Alloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        //public static IntPtr AllocMidiHeader()
        //{
        //    return Marshal.AllocHGlobal(SizeOfMidiHeader);
        //}

        public static T Unpack<T>(IntPtr memory)
        {
            return (T)Marshal.PtrToStructure(memory, typeof(T));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void Pack<T>(T data, IntPtr memory)
        {
            Marshal.StructureToPtr(data, memory, false);
        }

        public static readonly int SizeOfMidiHeader = SizeOf(typeof(MidiHeader));
        public static readonly int SizeOfMidiInCaps = SizeOf(typeof(MidiInCaps));
        public static readonly int SizeOfMidiOutCaps = SizeOf(typeof(MidiOutCaps));
        public static readonly int SizeOfMmTime = SizeOf(typeof(MmTime));
        public static readonly int SizeOfMidiStreamOutPortProperty = SizeOf(typeof(MidiStreamOutPortProperty));
    }
}