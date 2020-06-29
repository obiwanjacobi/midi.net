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
                _timeDivision = newTimeDivision.Value;

                // test for smpte time
                if (((short)_timeDivision) < 0)
                {
                    var fps = SmpteTime.ToFrameRate(Math.Abs((sbyte)((_timeDivision & 0xFF00) >> 8)));
                    var subsPerFrame = _timeDivision & 0x00FF;

                    _smpte = new SmpteTimeBase(fps, subsPerFrame);
                    _ppqn = 0;
                }
                else
                {
                    ThrowIfNotAMultipleOf24(newTimeDivision.Value, "TimeDivision");

                    _ppqn = _timeDivision;
                    _smpte = null;
                }
            }

            if (newPpqn.HasValue)
            {
                Check.IfArgumentOutOfRange(newPpqn.Value, 0, ushort.MaxValue, "PulsesPerQuarterNote");
                ThrowIfNotAMultipleOf24(newPpqn.Value, "PulsesPerQuarterNote");

                _ppqn = newPpqn.Value;
                _timeDivision = _ppqn;
                _smpte = null;
            }

            if (newSmpte != null)
            {
                _timeDivision = (-(SmpteTime.FromFrameRate(newSmpte.FramesPerSecond) << 8)) | (newSmpte.SubFramesPerFrame & 0xFF);
                _ppqn = 0;
                _smpte = newSmpte;
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
        private int _ppqn;

        /// <summary>
        /// Gets or sets how many pulses per quarter note (PPQN).
        /// </summary>
        /// <remarks>Same as TimeDivision (but never negative). Must be multiples of 24.</remarks>
        public int PulsesPerQuarterNote
        {
            get { return _ppqn; }
            set { OnValueChanged(null, value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="TimeDivision"/> property.
        /// </summary>
        private int _timeDivision;

        /// <summary>
        /// Gets or sets the Midi file time division value (MThd).
        /// </summary>
        /// <remarks>Same as PPQN. When negative a SMPTE time is used.</remarks>
        public int TimeDivision
        {
            get { return _timeDivision; }
            set { OnValueChanged(value, null, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="Smpte"/> property.
        /// </summary>
        private SmpteTimeBase _smpte;

        /// <summary>
        /// Gets or sets the Smpte time base values.
        /// </summary>
        public SmpteTimeBase Smpte
        {
            get { return _smpte; }
            set { OnValueChanged(null, null, value); }
        }
    }
}