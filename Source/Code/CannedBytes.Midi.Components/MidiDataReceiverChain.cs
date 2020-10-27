namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverChain class provides a chaining implementation for receiver chain components.
    /// </summary>
    public abstract class MidiDataReceiverChain : DisposableBase, IChainOf<IMidiDataReceiver>
    {
        /// <summary>
        /// Call to relay the short midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextReceiverShortData(int data, long timestamp)
        {
            if (Successor != null)
            {
                Successor.ShortData(data, timestamp);
            }
        }

        /// <summary>
        /// Call to relay the long midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="stream">The long midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextReceiverLongData(IMidiStream stream, long timestamp)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            if (Successor != null)
            {
                Successor.LongData(stream, timestamp);
            }
        }

        /// <summary>
        /// Gets or sets the next receiver component this instance will call.
        /// </summary>
        public IMidiDataReceiver Successor { get; set; }
    }
}