namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// The SmpteTime class contains time in a SMPTE format.
    /// </summary>
    /// <remarks>After construction the class is immutable.</remarks>
    public class SmpteTime
    {
        /// <summary>Number microseconds per hour.</summary>
        public const long MicrosecondsInHour = 3600000000L;

        /// <summary>Number microseconds per minute.</summary>
        public const long MicrosecondsInMinute = 60000000L;

        /// <summary>Number microseconds per second.</summary>
        public const long MicrosecondsInSecond = 1000000L;

        /// <summary>Number microseconds per millisecond.</summary>
        public const long MicrosecondsInMillisecond = 1000L;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="hour">Smpte hours.</param>
        /// <param name="minute">Smpte minutes.</param>
        /// <param name="second">Smpte seconds.</param>
        /// <param name="frame">Smpte frames.</param>
        /// <param name="fps">Smpte frames per second. Must not be <see cref="SmpteFrameRate.None"/> or <see cref="SmpteFrameRate.SmpteDrop30"/>.</param>
        public SmpteTime(int hour, int minute, int second, int frame, SmpteFrameRate fps)
        {
            ThrowIfInvalidFrameRate(fps);

            Hour = hour;
            Minute = minute;
            Second = second;
            Frame = frame;
            FramesPerSecond = fps;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="hour">Smpte hours.</param>
        /// <param name="minute">Smpte minutes.</param>
        /// <param name="second">Smpte seconds.</param>
        /// <param name="frame">Smpte frames.</param>
        /// <param name="subFrames">Smpte sub-frames.</param>
        /// <param name="fps">Smpte frames per second. Must not be <see cref="SmpteFrameRate.None"/> or <see cref="SmpteFrameRate.SmpteDrop30"/>.</param>
        /// <param name="subFramesPerFrame">The number of sub-frames in a frame. Can be zero.</param>
        public SmpteTime(int hour, int minute, int second, int frame, int subFrames, SmpteFrameRate fps, int subFramesPerFrame)
        {
            ThrowIfInvalidFrameRate(fps);

            Hour = hour;
            Minute = minute;
            Second = second;
            Frame = frame;
            SubFrames = subFrames;
            FramesPerSecond = fps;
            SubFramesPerFrame = subFramesPerFrame;
        }

        /// <summary>
        /// Constructs a new instance based on <paramref name="microseconds"/>.
        /// </summary>
        /// <param name="microseconds">A number of microseconds.</param>
        /// <param name="fps">The frame rate to use.</param>
        /// <param name="subFramesPerFrame">The number of sub-frames in a frame. Can be zero.</param>
        /// <returns>Never returns null.</returns>
        public static SmpteTime FromMicroseconds(long microseconds, SmpteFrameRate fps, int subFramesPerFrame)
        {
            ThrowIfInvalidFrameRate(fps);

            int frameRate = FromFrameRate(fps);
            float microsPerFrame = GetMicrosecondsPerFrame(frameRate);

            int hour = (int)(microseconds / MicrosecondsInHour);
            microseconds -= hour * MicrosecondsInHour;
            int minutes = (int)(microseconds / MicrosecondsInMinute);
            microseconds -= minutes * MicrosecondsInMinute;
            int seconds = (int)(microseconds / MicrosecondsInSecond);
            microseconds -= seconds * MicrosecondsInSecond;
            int frames = (int)(microseconds / microsPerFrame);
            microseconds -= (int)(frames * microsPerFrame);

            int subFrames = 0;
            if (subFramesPerFrame > 0)
            {
                subFrames = (int)(microseconds / (microsPerFrame / subFramesPerFrame));
            }

            return new SmpteTime(hour, minutes, seconds, frames, subFrames, fps, subFramesPerFrame);
        }

        /// <summary>
        /// Calculates the number of microseconds for the current time.
        /// </summary>
        /// <returns>Returns a value in microseconds.</returns>
        public long ToMicroseconds()
        {
            long microsecs = Hour * MicrosecondsInHour;
            microsecs += Minute * MicrosecondsInMinute;
            microsecs += Second * MicrosecondsInSecond;
            microsecs += (long)((float)Frame * MicrosecondsPerFrame);
            microsecs += (long)((float)SubFrames * (MicrosecondsPerFrame * (float)SubFramesPerFrame));

            return microsecs;
        }

        /// <summary>
        /// Converts the current instance to a new Smpte time base.
        /// </summary>
        /// <param name="fps">Frames per second.</param>
        /// <param name="subFramesPerFrame">Sub-frames per frame.</param>
        /// <returns>Returns a new instance.</returns>
        public SmpteTime ConvertTo(SmpteFrameRate fps, int subFramesPerFrame)
        {
            long microsecs = ToMicroseconds();

            return FromMicroseconds(microsecs, fps, subFramesPerFrame == 0 ? SubFramesPerFrame : subFramesPerFrame);
        }

        /// <summary>
        /// Convert an int with value 24, 25, 29 and 30 to its corresponding <see cref="SmpteFrameRate"/> value.
        /// </summary>
        /// <param name="frameRate">The frameRate value.</param>
        /// <returns>Returns <see cref="SmpteFrameRate.None"/> of no conversion could be made.</returns>
        public static SmpteFrameRate ToFrameRate(int frameRate)
        {
            SmpteFrameRate fps = SmpteFrameRate.None;

            if (Enum.IsDefined(typeof(SmpteFrameRate), frameRate))
            {
                fps = (SmpteFrameRate)frameRate;
            }

            //switch (frameRate)
            //{
            //    case 24:
            //        fps = SmpteFrameRate.Smpte24;
            //        break;
            //    case 25:
            //        fps = SmpteFrameRate.Smpte25;
            //        break;
            //    case 29:
            //        fps = SmpteFrameRate.SmpteDrop30;
            //        break;
            //    case 30:
            //        fps = SmpteFrameRate.Smpte30;
            //        break;
            //}

            return fps;
        }

        /// <summary>
        /// Converts a frame rate value to an integer.
        /// </summary>
        /// <param name="fps">The frame rate value.</param>
        /// <returns>Returns zero if <see cref="SmpteFrameRate.None"/> was specified.</returns>
        public static int FromFrameRate(SmpteFrameRate fps)
        {
            int frameRate = (int)fps;

            //switch (fps)
            //{
            //    case SmpteFrameRate.Smpte24:
            //        frameRate = 24;
            //        break;
            //    case SmpteFrameRate.Smpte25:
            //        frameRate = 25;
            //        break;
            //    case SmpteFrameRate.SmpteDrop30:
            //        frameRate = 29;
            //        break;
            //    case SmpteFrameRate.Smpte30:
            //        frameRate = 30;
            //        break;
            //}

            return frameRate;
        }

        /// <summary>
        /// Gets the Smpte hours.
        /// </summary>
        public int Hour { get; private set; }

        /// <summary>
        /// Gets the Smpte minutes.
        /// </summary>
        public int Minute { get; private set; }

        /// <summary>
        /// Gets the Smpte seconds.
        /// </summary>
        public int Second { get; private set; }

        /// <summary>
        /// Gets the Smpte frames.
        /// </summary>
        public int Frame { get; private set; }

        /// <summary>
        /// Gets the Smpte sub-frames.
        /// </summary>
        public int SubFrames { get; private set; }

        /// <summary>
        /// Gets the Smpte frames per second.
        /// </summary>
        public SmpteFrameRate FramesPerSecond { get; private set; }

        /// <summary>
        /// Gets the number of sub-frames per frame.
        /// </summary>
        public int SubFramesPerFrame { get; private set; }

        /// <summary>
        /// Gets the number of microseconds for one frame.
        /// </summary>
        public float MicrosecondsPerFrame
        {
            get { return GetMicrosecondsPerFrame(FromFrameRate(FramesPerSecond)); }
        }

        /// <summary>
        /// Returns the number of microseconds per frame.
        /// </summary>
        /// <param name="frameRate">The frame rate (per second) to use. Use 29.97 for drop 30.</param>
        /// <returns>Returns the value in microseconds.</returns>
        private static float GetMicrosecondsPerFrame(float frameRate)
        {
            return MicrosecondsInSecond / frameRate;
        }

        /// <summary>
        /// Throws an exception if the specified <paramref name="fps"/> is not supported.
        /// </summary>
        /// <param name="fps">Must not be <see cref="SmpteFrameRate.None"/> or <see cref="SmpteFrameRate.SmpteDrop30"/>.</param>
        private static void ThrowIfInvalidFrameRate(SmpteFrameRate fps)
        {
            // drop-30 is not supported.
            if (fps == SmpteFrameRate.None || fps == SmpteFrameRate.SmpteDrop30)
            {
                throw new ArgumentException("Invalid Frames per Second value.", nameof(fps));
            }
        }
    }
}