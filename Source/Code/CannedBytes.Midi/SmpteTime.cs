namespace CannedBytes.Midi
{
    /// <summary>
    /// The SmpteTime class contains time in a smpte format.
    /// </summary>
    /// <remarks>After construction the class is immutable.</remarks>
    public class SmpteTime
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
            this.Hour = hour;
            this.Minute = minute;
            this.Second = second;
            this.Frame = frame;
            this.FramesPerSecond = fps;
        }

        /// <summary>
        /// Smpte hours.
        /// </summary>
        public int Hour { get; private set; }

        /// <summary>
        /// Smpte minutes.
        /// </summary>
        public int Minute { get; private set; }

        /// <summary>
        /// Smpte seconds.
        /// </summary>
        public int Second { get; private set; }

        /// <summary>
        /// Smpte frames.
        /// </summary>
        public int Frame { get; private set; }

        /// <summary>
        /// Smpte frames per second.
        /// </summary>
        public int FramesPerSecond { get; private set; }
    }
}