using System;
using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The SmpteTime structure contains time in a smpte format.
    /// </summary>
    /// <remarks>After construction the structure is immutable.</remarks>
    public struct SmpteTime
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="hour">Smpte hours.</param>
        /// <param name="minute">Smpte minutes.</param>
        /// <param name="second">Smpte seconds.</param>
        /// <param name="frame">Smpte frames.</param>
        /// <param name="fps">Smpte frames per second.</param>
        public SmpteTime(int hour, int minute, int second, int frame, int fps)
        {
            _hour = hour;
            _minute = minute;
            _second = second;
            _frame = frame;
            _fps = fps;
        }

        private int _hour;

        /// <summary>
        /// Smpte hours.
        /// </summary>
        public int Hour
        {
            get { return _hour; }
        }

        private int _minute;

        /// <summary>
        /// Smpte minutes.
        /// </summary>
        public int Minute
        {
            get { return _minute; }
        }

        private int _second;

        /// <summary>
        /// Smpte seconds.
        /// </summary>
        public int Second
        {
            get { return _second; }
        }

        private int _frame;

        /// <summary>
        /// Smpte frames.
        /// </summary>
        public int Frame
        {
            get { return _frame; }
        }

        private int _fps;

        /// <summary>
        /// Smpte frames per second.
        /// </summary>
        public int FramesPerSecond
        {
            get { return _fps; }
        }
    }

    /// <summary>
    /// Internal time structure passed to <see cref="NativeMethods"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct MmTime
    {
        [FieldOffset(0)]
        public UInt32 wType;
        [FieldOffset(4)]
        public UInt32 ms;
        [FieldOffset(4)]
        public UInt32 sample;
        [FieldOffset(4)]
        public UInt32 cb;
        [FieldOffset(4)]
        public UInt32 ticks;
        [FieldOffset(4)]
        public UInt32 midiSongPtrPos;
        [FieldOffset(4)]
        public Byte smpteHour;
        [FieldOffset(5)]
        public Byte smpteMin;
        [FieldOffset(6)]
        public Byte smpteSec;
        [FieldOffset(7)]
        public Byte smpteFrame;
        [FieldOffset(8)]
        public Byte smpteFps;
        [FieldOffset(9)]
        public Byte smpteDummy;
        [FieldOffset(10)]
        public Byte smptePad0;
        [FieldOffset(11)]
        public Byte smptePad1;
    }
}