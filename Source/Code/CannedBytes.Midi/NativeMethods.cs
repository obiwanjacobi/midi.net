using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The NativeMethods static class implements the P-invoke calls to the Win32
    /// midi API (midiXxxxx).
    /// </summary>
    internal static class NativeMethods
    {
        // Represents the method that handles messages from a midi driver.
        public delegate void MidiProc(IntPtr handle, uint msg, IntPtr instance,
            IntPtr param1, IntPtr param2);

        [DllImport("winmm.dll")]
        public static extern int midiInGetErrorText(int errCode,
            StringBuilder errMsg, int sizeOfErrMsg);

        [DllImport("winmm.dll")]
        public static extern int midiInOpen(out MidiInSafeHandle handle, uint deviceID,
            MidiProc proc, IntPtr instance, uint flags);

        [DllImport("winmm.dll")]
        public static extern int midiInClose(IntPtr handle);

        [DllImport("winmm.dll")]
        public static extern int midiInStart(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiInStop(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiInReset(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiInPrepareHeader(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiInUnprepareHeader(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiInAddBuffer(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiInGetDevCaps(IntPtr handle,
            ref MidiInCaps caps, uint sizeOfmidiInCaps);

        [DllImport("winmm.dll")]
        public static extern int midiInGetNumDevs();

        [DllImport("winmm.dll")]
        public static extern int midiConnect(MidiSafeHandle inHandle, MidiSafeHandle outHandle,
            IntPtr reserved);

        [DllImport("winmm.dll")]
        public static extern int midiDisconnect(MidiSafeHandle inHandle, MidiSafeHandle outHandle,
            IntPtr reserved);

        [DllImport("winmm.dll")]
        public static extern int midiOutOpen(out MidiOutSafeHandle handle, uint deviceID,
            MidiProc proc, IntPtr instance, uint flags);

        [DllImport("winmm.dll")]
        public static extern int midiOutClose(IntPtr handle);

        [DllImport("winmm.dll")]
        public static extern int midiOutReset(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiOutShortMsg(MidiSafeHandle handle, uint message);

        [DllImport("winmm.dll")]
        public static extern int midiOutPrepareHeader(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiOutUnprepareHeader(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiOutLongMsg(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiOutGetErrorText(int errCode,
            StringBuilder errMsg, int sizeOfErrMsg);

        [DllImport("winmm.dll")]
        public static extern int midiOutGetDevCaps(IntPtr handle,
            ref MidiOutCaps caps, uint sizeOfmidiOutCaps);

        [DllImport("winmm.dll")]
        public static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        public static extern int midiStreamClose(IntPtr handle);

        [DllImport("winmm.dll")]
        public static extern int midiStreamOpen(out MidiStreamOutSafeHandle handle, ref uint deviceID, uint cMidi,
            MidiProc proc, IntPtr instance, uint flags);

        [DllImport("winmm.dll")]
        public static extern int midiStreamOut(MidiSafeHandle handle,
            IntPtr header, uint sizeOfmidiHeader);

        [DllImport("winmm.dll")]
        public static extern int midiStreamPause(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiStreamRestart(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiStreamStop(MidiSafeHandle handle);

        [DllImport("winmm.dll")]
        public static extern int midiStreamPosition(MidiSafeHandle handle, ref MmTime time, uint sizeOfMmTime);

        [DllImport("winmm.dll")]
        public static extern int midiStreamProperty(MidiSafeHandle handle, ref MidiStreamOutPortProperty p, uint flags);

        public const int MAXERRORLENGTH = 256;
        public const int MMSYSERR_NOERROR = 0;
        public const int MIDIERR_STILLPLAYING = 65;

        public const uint CALLBACK_FUNCTION = 0x30000;
        public const uint MIDI_IO_STATUS = 32;
        public const uint MIM_OPEN = 0x3C1;
        public const uint MIM_CLOSE = 0x3C2;
        public const uint MIM_DATA = 0x3C3;
        public const uint MIM_LONGDATA = 0x3C4;
        public const uint MIM_ERROR = 0x3C5;
        public const uint MIM_LONGERROR = 0x3C6;
        public const uint MOM_OPEN = 0x3C7;
        public const uint MOM_CLOSE = 0x3C8;
        public const uint MOM_DONE = 0x3C9;
        public const uint MOM_POSITIONCB = 0x3CA;
        public const uint MIM_MOREDATA = 0x3CC;
        public const uint MHDR_DONE = 1;
        public const uint MHDR_PREPARED = 2;
        public const uint MHDR_INQUEUE = 4;
        public const uint MHDR_ISSTRM = 8;

        public const uint MM_STREAM_OPEN = 0x3D4;
        public const uint MM_STREAM_CLOSE = 0x3D5;
        public const uint MM_STREAM_DONE = 0x3D6;
        public const uint MM_STREAM_ERROR = 0x3D7;

        // stream properties
        public const uint MIDIPROP_SET = 0x80000000;
        public const uint MIDIPROP_GET = 0x40000000;
        public const uint MIDIPROP_TIMEDIV = 0x00000001;
        public const uint MIDIPROP_TEMPO = 0x00000002;
    }
}