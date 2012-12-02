namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverFilterOnStatus class filters incoming short midi messages based
    /// on their status byte.
    /// </summary>
    public sealed class MidiReceiverFilterOnStatus : MidiReceiverChain, IMidiReceiver
    {
        private byte _status;

        /// <summary>
        /// Gets or sets the status value to filter on
        /// </summary>
        public byte Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private bool PassFilter(int data)
        {
            return (MidiData.GetStatus(data) != Status);
        }

        /// <summary>
        /// Passes the short midi message to the next receiver component if it passes the filter.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void ShortData(int data, int timeIndex)
        {
            if (PassFilter(data))
            {
                NextReceiverShortData(data, timeIndex);
            }
        }

        /// <summary>
        /// Passes the long midi message to the next receiver component. No filtering is applied.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void LongData(MidiBufferStream buffer, int timeIndex)
        {
            NextReceiverLongData(buffer, timeIndex);
        }
    }
}