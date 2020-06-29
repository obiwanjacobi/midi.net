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
                _bpm = newBpm.Value;

                if (_bpm > 0.0F)
                {
                    _microTempo = (int)(MicrosecondsPerMinute / _bpm);
                    _milliTempo = _microTempo / 1000;
                }
                else
                {
                    Clear();
                }
            }

            if (newMilliTempo.HasValue)
            {
                _milliTempo = newMilliTempo.Value;

                if (_milliTempo > 0)
                {
                    _microTempo = _milliTempo * 1000;
                    _bpm = MicrosecondsPerMinute / (float)_microTempo;
                }
                else
                {
                    Clear();
                }
            }

            if (newMicroTempo.HasValue)
            {
                _microTempo = newMicroTempo.Value;

                if (_microTempo > 0)
                {
                    _milliTempo = _microTempo / 1000;
                    _bpm = MicrosecondsPerMinute / (float)_microTempo;
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
            _milliTempo = 0;
            _microTempo = 0;
            _bpm = 0.0F;
        }

        /// <summary>
        /// Backing field for the <see cref="BeatsPerMinute"/> property.
        /// </summary>
        private float _bpm;

        /// <summary>
        /// Gets or sets the BPM.
        /// </summary>
        public float BeatsPerMinute
        {
            get { return _bpm; }
            set { OnValueChanged(value, null, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MicrosecondTempo"/> property.
        /// </summary>
        private int _microTempo;

        /// <summary>
        /// Gets or sets the tempo in microseconds per quarter note.
        /// </summary>
        /// <remarks>Midi file compatible tempo value (Meta Event).</remarks>
        public int MicrosecondTempo
        {
            get { return _microTempo; }
            set { OnValueChanged(null, value, null); }
        }

        /// <summary>
        /// Backing field for the <see cref="MillisecondTempo"/> property.
        /// </summary>
        private int _milliTempo;

        /// <summary>
        /// Gets or sets the tempo in milliseconds per quarter note.
        /// </summary>
        public int MillisecondTempo
        {
            get { return _milliTempo; }
            set { OnValueChanged(null, null, value); }
        }
    }
}