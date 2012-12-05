namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverChain class provides a chaining implementation for receiver chain components.
    /// </summary>
    public abstract class MidiReceiverChain : DisposableBase, IChainOf<IMidiDataReceiver>
    {
        /// <summary>
        /// Call to relay the short midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="NextReceiver"/> is not set.</remarks>
        protected void NextReceiverShortData(int data, int timeIndex)
        {
            if (Next != null)
            {
                Next.ShortData(data, timeIndex);
            }
        }

        /// <summary>
        /// Call to relay the long midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="NextReceiver"/> is not set.</remarks>
        protected void NextReceiverLongData(MidiBufferStream buffer, int timeIndex)
        {
            if (Next != null)
            {
                Next.LongData(buffer, timeIndex);
            }
        }

        private IMidiDataReceiver _receiver;

        /// <summary>
        /// Gets or sets the next receiver component this instance will call.
        /// </summary>
        public IMidiDataReceiver Next
        {
            get { return _receiver; }
            set { _receiver = value; }
        }
    }
}