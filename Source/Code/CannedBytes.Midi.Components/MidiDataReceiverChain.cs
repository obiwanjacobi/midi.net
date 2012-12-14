namespace CannedBytes.Midi.Components
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The MidiReceiverChain class provides a chaining implementation for receiver chain components.
    /// </summary>
    public abstract class MidiDataReceiverChain : DisposableBase, IChainOf<IMidiDataReceiver>
    {
        /// <summary>
        /// Call to relay the short midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextReceiverShortData(int data, int timeIndex)
        {
            if (this.Successor != null)
            {
                this.Successor.ShortData(data, timeIndex);
            }
        }

        /// <summary>
        /// Call to relay the long midi message data to the next receiver component in the chain.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextReceiverLongData(MidiBufferStream buffer, int timeIndex)
        {
            Contract.Requires(buffer != null);
            Check.IfArgumentNull(buffer, "buffer");

            if (this.Successor != null)
            {
                this.Successor.LongData(buffer, timeIndex);
            }
        }

        /// <summary>
        /// Gets or sets the next receiver component this instance will call.
        /// </summary>
        public IMidiDataReceiver Successor { get; set; }
    }
}