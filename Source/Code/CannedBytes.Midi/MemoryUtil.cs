using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Security;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Helper class for unmanaged memory operations.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class MemoryUtil
    {
        public static int SizeOf(Type type)
        {
            Contract.Requires(type != null);

            return Marshal.SizeOf(type);
        }

        public static void Free(IntPtr memory)
        {
            Contract.Requires(memory != IntPtr.Zero);

            Marshal.FreeHGlobal(memory);
        }

        public static IntPtr Alloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        //public static T Unpack<T>(IntPtr memory)
        //{
        //    return (T)Marshal.PtrToStructure(memory, typeof(T));
        //}

        //public static void Pack<T>(T data, IntPtr memory)
        //{
        //    Marshal.StructureToPtr(data, memory, false);
        //}

        public static readonly int SizeOfMidiHeader = SizeOf(typeof(MidiHeader));
        public static readonly int SizeOfMidiInCaps = SizeOf(typeof(MidiInCaps));
        public static readonly int SizeOfMidiOutCaps = SizeOf(typeof(MidiOutCaps));
        public static readonly int SizeOfMmTime = SizeOf(typeof(MmTime));
        public static readonly int SizeOfMidiStreamOutPortProperty = SizeOf(typeof(MidiOutStreamPortProperty));
    }
}