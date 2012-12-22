namespace CannedBytes.Midi
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Represents a tempo-related time base.
    /// </summary>
    public class MidiRelativeTimeBase
    {
        /// <summary>
        /// Called when one of the properties change value.
        /// </summary>
        /// <param name="newTimeDivision">Non-null when changed.</param>
        /// <param name="newPpqn">Non-null when changed.</param>
        /// <param name="newSmpte">Non-null when changed.</param>
        protected virtual void OnValueChanged(int? newTimeDivision, int? newPpqn, SmpteTimeBase newSmpte)
        {
            if (newTimeDivision.HasValue)
            {
                this.timeDivision = newTimeDivision.Value;

                // test for smpte time
                if (((short)this.timeDivision) < 0)
                {
                    var fps = SmpteTime.ToFrameRate(Math.Abs((sbyte)((this.timeDivision & 0xFF00) >> 8)));
                    var subsPerFrame = this.timeDivision & 0x00FF;

                    this.smpte = new SmpteTimeBase(fps, subsPerFrame);
                    this.ppqn = 0;
                }
                else
                {
                    ThrowIfNotAMultipleOf24(newTimeDivision.Value, "TimeDivision");

                    this.ppqn = this.timeDivision;
                    this.smpte = null;
                }
            }

            if (newPpqn.HasValue)
            {
                Check.IfArgumentOutOfRange(newPpqn.Value, 0, ushort.MaxValue, "PulsesPerQuarterNote");
                ThrowIfNotAMultipleOf24(newPpqn.Value, "PulsesPerQuarterNote");

                this.ppqn = newPpqn.Value;
                this.timeDivision = this.ppqn;
                this.smpte = null;
            }

            if (newSmpte != null)
            {
                this.timeDivision = (-(SmpteTime.FromFrameRate(newSmpte.FramesPerSecond) << 8)) | (newSmpte.SubFramesPerFrame & 0xFF);
                this.ppqn = 0;
                this.smpte = newSmpte;
            }
        }

        /// <summary>
        /// Throws an exception when the <paramref name="value"/> is not a multiple of 24.
        /// </summary>
        /// <param name="value">Can be zero.</param>
        /// <param name="argumentName">The name of the method argument that contained the <paramref name="value"/>.</param>
        private static void ThrowIfNotAMultipleOf24(int value, string argumentName)
        {
            if (value % 24 > 0)
            {
                var msg = String.Format(CultureInfo.InvariantCulture, "The value {0} is not a multiple of 24.", value);
                throw new ArgumentException(msg, argumentName);
            }
        }

        /// <summary>
        /// Backing field for the <see cref="P:PulsesPerQuarterNote"/> property.
        /// </summary>
        private int ppqn;

        /// <summary>
        /// Gets or sets how many pulses per quarter note (PPQN).
        /// </summary>
        /// <remarks>Same as TimeDivision (but never negative). Must be multiples of 24.</remarks>
        public int PulsesPerQuarterNote
        {
            get { return this.ppqn; }
            set { this.OnValueChanged(null, value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="TimeDivision"/> property.
        /// </summary>
        private int timeDivision;

        /// <summary>
        /// Gets or sets the Midi file time division value (MThd).
        /// </summary>
        /// <remarks>Same as PPQN. When negative a SMPTE time is used.</remarks>
        public int TimeDivision
        {
            get { return this.timeDivision; }
            set { this.OnValueChanged(value, null, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="Smpte"/> property.
        /// </summary>
        private SmpteTimeBase smpte;

        /// <summary>
        /// Gets or sets the Smpte time base values.
        /// </summary>
        public SmpteTimeBase Smpte
        {
            get { return this.smpte; }
            set { this.OnValueChanged(null, null, value); }
        }
    }
}