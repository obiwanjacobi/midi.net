namespace CannedBytes.Midi
{
    /// <summary>
    /// A musical tempo indication.
    /// </summary>
    public class MidiTempo
    {
        /// <summary>
        /// The number of microseconds that are in a minute (BPM).
        /// </summary>
        private const float MicrosecondsPerMinute = 60000000.0F;

        /// <summary>
        /// Called when one of the property values changes.
        /// </summary>
        /// <param name="newBpm">Non-null when changed.</param>
        /// <param name="newMilliTempo">Non-null when changed.</param>
        /// <param name="newMicroTempo">Non-null when changed.</param>
        protected virtual void OnValueChanged(float? newBpm, int? newMilliTempo, int? newMicroTempo)
        {
            if (newBpm.HasValue)
            {
                this.bpm = newBpm.Value;

                if (this.bpm > 0.0F)
                {
                    this.microTempo = (int)(MicrosecondsPerMinute / this.bpm);
                    this.milliTempo = this.microTempo / 1000;
                }
                else
                {
                    this.Clear();
                }
            }

            if (newMilliTempo.HasValue)
            {
                this.milliTempo = newMilliTempo.Value;

                if (this.milliTempo > 0)
                {
                    this.microTempo = this.milliTempo * 1000;
                    this.bpm = MicrosecondsPerMinute / (float)this.microTempo;
                }
                else
                {
                    this.Clear();
                }
            }

            if (newMicroTempo.HasValue)
            {
                this.microTempo = newMicroTempo.Value;

                if (this.microTempo > 0)
                {
                    this.milliTempo = this.microTempo / 1000;
                    this.bpm = MicrosecondsPerMinute / (float)this.microTempo;
                }
                else
                {
                    this.Clear();
                }
            }
        }

        /// <summary>
        /// Resets all property values.
        /// </summary>
        public void Clear()
        {
            this.milliTempo = 0;
            this.microTempo = 0;
            this.bpm = 0.0F;
        }

        /// <summary>
        /// Backing field for the <see cref="BeatsPerMinute"/> property.
        /// </summary>
        private float bpm;

        /// <summary>
        /// Gets or sets the BPM.
        /// </summary>
        public float BeatsPerMinute
        {
            get { return this.bpm; }
            set { this.OnValueChanged(value, null, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MicrosecondTempo"/> property.
        /// </summary>
        private int microTempo;

        /// <summary>
        /// Gets or sets the tempo in microseconds per quarter note.
        /// </summary>
        /// <remarks>Midi file compatible tempo value (Meta Event).</remarks>
        public int MicrosecondTempo
        {
            get { return this.microTempo; }
            set { this.OnValueChanged(null, value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MillisecondTempo"/> property.
        /// </summary>
        private int milliTempo;

        /// <summary>
        /// Gets or sets the tempo in milliseconds per quarter note.
        /// </summary>
        public int MillisecondTempo
        {
            get { return this.milliTempo; }
            set { this.OnValueChanged(null, null, value); }
        }
    }
}