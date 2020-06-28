namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
            Check.IfArgumentOutOfRange(timeDivision, short.MinValue, ushort.MaxValue, "timeDivision");
            if (timeDivision == 0)
            {
                throw new ArgumentException("The timeDivision value can not be zero.", "timeDivision");
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Check is not recognized.")]
        public static MidiTimeBase Create(MidiRelativeTimeBase timeBase, MidiTempo tempo)
        {
            Check.IfArgumentNull(timeBase, "timeBase");
            Check.IfArgumentNull(tempo, "tempo");
            Check.IfArgumentOutOfRange(tempo.MillisecondTempo, 0, int.MaxValue, "tempo.MillisecondTempo");

            var midiTimeBase = new MidiTimeBase();

            midiTimeBase.MillisecondResolution = tempo.MillisecondTempo / timeBase.PulsesPerQuarterNote;

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
                this.milliResolution = newMilliResolution.Value;
                this.microResolution = this.milliResolution * 1000;
            }

            if (newMicroResolution.HasValue)
            {
                this.microResolution = newMicroResolution.Value;
                this.milliResolution = this.microResolution / 1000;
            }

            this.smpteTime = null;
        }

        /// <summary>
        /// Backing field for the <see cref="MillisecondResolution"/> property.
        /// </summary>
        private long milliResolution;

        /// <summary>
        /// Gets or sets the resolution of the time base: how many ticks elapse per millisec.
        /// </summary>
        public long MillisecondResolution
        {
            get { return this.milliResolution; }
            set { this.OnValueChanged(value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MicrosecondResolution"/> property.
        /// </summary>
        private long microResolution;

        /// <summary>
        /// Gets or sets the resolution of the time base: how many ticks elapse per microsec.
        /// </summary>
        public long MicrosecondResolution
        {
            get { return this.microResolution; }
            set { this.OnValueChanged(null, value); }
        }

        /// <summary>
        /// Backing field for the <see cref="SmpteTime"/> property.
        /// </summary>
        private SmpteTime smpteTime;

        /// <summary>
        /// Gets or sets the SMPTE Time resolution information.
        /// </summary>
        public SmpteTime SmpteTime
        {
            get
            {
                if (this.smpteTime == null)
                {
                    this.smpteTime = SmpteTime.FromMicroseconds(this.microResolution, SmpteFrameRate.Smpte25, 40);
                }

                return this.smpteTime;
            }

            set
            {
                this.smpteTime = value;

                if (this.smpteTime != null)
                {
                    this.MicrosecondResolution = this.smpteTime.ToMicroseconds();
                }
                else
                {
                    this.MicrosecondResolution = 0;
                }
            }
        }
    }
}