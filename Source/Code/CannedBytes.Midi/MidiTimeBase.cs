namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    ///  Absolute Timer time base.
    /// </summary>
    public class MidiTimeBase
    {
        /// <summary>
        /// Factory method for Midi file compatible values.
        /// </summary>
        /// <param name="timeDivision">Must not be zero.</param>
        /// <param name="tempo">Can be zero.</param>
        /// <returns>Never returns null.</returns>
        public static MidiTimeBase Create(int timeDivision, int tempo)
        {
            Check.IfArgumentOutOfRange(timeDivision, short.MinValue, ushort.MaxValue, nameof(timeDivision));
            if (timeDivision == 0)
            {
                throw new ArgumentException("The timeDivision value can not be zero.", nameof(timeDivision));
            }

            var midiTimeBase = new MidiTimeBase();

            if (((short)timeDivision) < 0)
            {
                var fps = SmpteTime.ToFrameRate(Math.Abs((sbyte)((timeDivision & 0xFF00) >> 8)));
                var frames = timeDivision & 0x00FF;

                midiTimeBase.SmpteTime = new SmpteTime(0, 0, 0, frames, fps);
            }
            else
            {
                midiTimeBase.MillisecondResolution = tempo / timeDivision;
            }

            return midiTimeBase;
        }

        /// <summary>
        /// Factory method for use with relative <paramref name="timeBase"/> and <paramref name="tempo"/>.
        /// </summary>
        /// <param name="timeBase">Must not be null.</param>
        /// <param name="tempo">Must not be null.</param>
        /// <returns>Never returns null.</returns>
        public static MidiTimeBase Create(MidiRelativeTimeBase timeBase, MidiTempo tempo)
        {
            Check.IfArgumentNull(timeBase, nameof(timeBase));
            Check.IfArgumentNull(tempo, nameof(tempo));
            Check.IfArgumentOutOfRange(tempo.MillisecondTempo, 0, int.MaxValue, nameof(tempo.MillisecondTempo));

            var midiTimeBase = new MidiTimeBase
            {
                MillisecondResolution = tempo.MillisecondTempo / timeBase.PulsesPerQuarterNote
            };

            return midiTimeBase;
        }

        /// <summary>
        /// Called when one of the property values have changed.
        /// </summary>
        /// <param name="newMilliResolution">Non-null when changed.</param>
        /// <param name="newMicroResolution">Non-null when changed.</param>
        protected virtual void OnValueChanged(long? newMilliResolution, long? newMicroResolution)
        {
            if (newMilliResolution.HasValue)
            {
                _milliResolution = newMilliResolution.Value;
                _microResolution = _milliResolution * 1000;
            }

            if (newMicroResolution.HasValue)
            {
                _microResolution = newMicroResolution.Value;
                _milliResolution = _microResolution / 1000;
            }

            _smpteTime = null;
        }

        /// <summary>
        /// Backing field for the <see cref="MillisecondResolution"/> property.
        /// </summary>
        private long _milliResolution;

        /// <summary>
        /// Gets or sets the resolution of the time base: how many ticks elapse per millisec.
        /// </summary>
        public long MillisecondResolution
        {
            get { return _milliResolution; }
            set { OnValueChanged(value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MicrosecondResolution"/> property.
        /// </summary>
        private long _microResolution;

        /// <summary>
        /// Gets or sets the resolution of the time base: how many ticks elapse per microsec.
        /// </summary>
        public long MicrosecondResolution
        {
            get { return _microResolution; }
            set { OnValueChanged(null, value); }
        }

        /// <summary>
        /// Backing field for the <see cref="SmpteTime"/> property.
        /// </summary>
        private SmpteTime _smpteTime;

        /// <summary>
        /// Gets or sets the SMPTE Time resolution information.
        /// </summary>
        public SmpteTime SmpteTime
        {
            get
            {
                if (_smpteTime == null)
                {
                    _smpteTime = SmpteTime.FromMicroseconds(_microResolution, SmpteFrameRate.Smpte25, 40);
                }

                return _smpteTime;
            }

            set
            {
                _smpteTime = value;

                if (_smpteTime != null)
                {
                    MicrosecondResolution = _smpteTime.ToMicroseconds();
                }
                else
                {
                    MicrosecondResolution = 0;
                }
            }
        }
    }
}