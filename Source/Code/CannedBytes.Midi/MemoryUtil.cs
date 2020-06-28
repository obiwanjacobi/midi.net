namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Helper class for unmanaged memory operations.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class MemoryUtil
    {
        /// <summary>
        /// Returns the native size of a <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Must not be null.</param>
        /// <returns>Returns a positive integer.</returns>
        public static int SizeOf(Type type)
        {
            return Marshal.SizeOf(type);
        }

        /// <summary>
        /// Free's the unmanaged memory allocated by <see cref="M:Alloc"/>.
        /// </summary>
        /// <param name="memory">Memory pointer. Must not be IntPtr.Zero.</param>
        public static void Free(IntPtr memory)
        {
            Marshal.FreeHGlobal(memory);
        }

        /// <summary>
        /// Allocates unmanaged memory of <paramref name="size"/> bytes.
        /// </summary>
        /// <param name="size">A positive integer.</param>
        /// <returns>Returns a pointer to the memory.</returns>
        public static IntPtr Alloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        /// <summary>The native size of <see cref="MidiHeader"/>.</summary>
        public static readonly int SizeOfMidiHeader = SizeOf(typeof(MidiHeader));

        /// <summary>The native size of <see cref="MidiInCaps"/>.</summary>
        public static readonly int SizeOfMidiInCaps = SizeOf(typeof(MidiInCaps));

        /// <summary>The native size of <see cref="MidiOutCaps"/>.</summary>
        public static readonly int SizeOfMidiOutCaps = SizeOf(typeof(MidiOutCaps));

        /// <summary>The native size of <see cref="MmTime"/>.</summary>
        public static readonly int SizeOfMmTime = SizeOf(typeof(MmTime));

        /// <summary>The native size of <see cref="MidiOutStreamPortProperty"/>.</summary>
        public static readonly int SizeOfMidiStreamOutPortProperty = SizeOf(typeof(MidiOutStreamPortProperty));
    }
}