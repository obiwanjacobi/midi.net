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
                bpm = newBpm.Value;

                if (bpm > 0.0F)
                {
                    microTempo = (int)(MicrosecondsPerMinute / bpm);
                    milliTempo = microTempo / 1000;
                }
                else
                {
                    Clear();
                }
            }

            if (newMilliTempo.HasValue)
            {
                milliTempo = newMilliTempo.Value;

                if (milliTempo > 0)
                {
                    microTempo = milliTempo * 1000;
                    bpm = MicrosecondsPerMinute / (float)microTempo;
                }
                else
                {
                    Clear();
                }
            }

            if (newMicroTempo.HasValue)
            {
                microTempo = newMicroTempo.Value;

                if (microTempo > 0)
                {
                    milliTempo = microTempo / 1000;
                    bpm = MicrosecondsPerMinute / (float)microTempo;
                }
                else
                {
                    Clear();
                }
            }
        }

        /// <summary>
        /// Resets all property values.
        /// </summary>
        public void Clear()
        {
            milliTempo = 0;
            microTempo = 0;
            bpm = 0.0F;
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
            get { return bpm; }
            set { OnValueChanged(value, null, null); }
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
            get { return microTempo; }
            set { OnValueChanged(null, value, null); }
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
            get { return milliTempo; }
            set { OnValueChanged(null, null, value); }
        }
    }
}