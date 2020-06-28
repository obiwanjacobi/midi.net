namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverFilterOnStatus class filters incoming short midi messages based
    /// on their status byte.
    /// </summary>
    public sealed class MidiDataReceiverFilterOnStatus : MidiDataReceiverChain, IMidiDataReceiver
    {
        /// <summary>
        /// Gets or sets the status value to filter on.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Indicates if the <paramref name="data"/> passes the status filter.
        /// </summary>
        /// <param name="data">The midi message data.</param>
        /// <returns>Returns true if the data passes the filter.</returns>
        private bool PassFilter(int data)
        {
            return MidiData.GetStatus(data) != Status;
        }

        /// <summary>
        /// Passes the short midi message to the next receiver component if it passes the filter.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void ShortData(int data, long timestamp)
        {
            if (PassFilter(data))
            {
                NextReceiverShortData(data, timestamp);
            }
        }

        /// <summary>
        /// Passes the long midi message to the next receiver component. No filtering is applied.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            Check.IfArgumentNull(buffer, nameof(buffer));

            NextReceiverLongData(buffer, timestamp);
        }

        /// <summary>
        /// Called when disposing the object instance.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            // no op
        }
    }
}