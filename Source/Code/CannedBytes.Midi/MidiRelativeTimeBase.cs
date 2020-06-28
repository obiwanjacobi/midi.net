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
                timeDivision = newTimeDivision.Value;

                // test for smpte time
                if (((short)timeDivision) < 0)
                {
                    var fps = SmpteTime.ToFrameRate(Math.Abs((sbyte)((timeDivision & 0xFF00) >> 8)));
                    var subsPerFrame = timeDivision & 0x00FF;

                    smpte = new SmpteTimeBase(fps, subsPerFrame);
                    ppqn = 0;
                }
                else
                {
                    ThrowIfNotAMultipleOf24(newTimeDivision.Value, "TimeDivision");

                    ppqn = timeDivision;
                    smpte = null;
                }
            }

            if (newPpqn.HasValue)
            {
                Check.IfArgumentOutOfRange(newPpqn.Value, 0, ushort.MaxValue, "PulsesPerQuarterNote");
                ThrowIfNotAMultipleOf24(newPpqn.Value, "PulsesPerQuarterNote");

                ppqn = newPpqn.Value;
                timeDivision = ppqn;
                smpte = null;
            }

            if (newSmpte != null)
            {
                timeDivision = (-(SmpteTime.FromFrameRate(newSmpte.FramesPerSecond) << 8)) | (newSmpte.SubFramesPerFrame & 0xFF);
                ppqn = 0;
                smpte = newSmpte;
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
            get { return ppqn; }
            set { OnValueChanged(null, value, null); }
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
            get { return timeDivision; }
            set { OnValueChanged(value, null, null); }
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
            get { return smpte; }
            set { OnValueChanged(null, null, value); }
        }
    }
}