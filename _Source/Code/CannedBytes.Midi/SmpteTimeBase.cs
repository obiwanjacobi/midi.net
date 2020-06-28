namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// Contains time base information for a <see cref="SmpteTime"/>.
    /// </summary>
    public class SmpteTimeBase
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="fps">Number of frames per second.</param>
        /// <param name="subsPerFrame">Number of sub-frames per frame. Can be zero.</param>
        public SmpteTimeBase(SmpteFrameRate fps, int subsPerFrame)
        {
            Check.IfArgumentOutOfRange(subsPerFrame, 0, int.MaxValue, "subsPerFrame");
            if (fps == SmpteFrameRate.None)
            {
                throw new ArgumentException("None is not a valid value.", "fps");
            }

            this.FramesPerSecond = fps;
            this.SubFramesPerFrame = subsPerFrame;
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        public SmpteFrameRate FramesPerSecond { get; private set; }

        /// <summary>
        /// Gets the number of sub-frames per frame.
        /// </summary>
        public int SubFramesPerFrame { get; private set; }
    }
}