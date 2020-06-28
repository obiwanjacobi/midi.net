namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiSenderChain class provides a chaining implementation for sender chain components.
    /// </summary>
    public abstract class MidiDataSenderChain : DisposableBase, IChainOf<IMidiDataSender>
    {
        /// <summary>
        /// Call to relay the short midi message data to the next sender component in the chain.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextSenderShortData(int data)
        {
            if (Successor != null)
            {
                Successor.ShortData(data);
            }
        }

        /// <summary>
        /// Call to relay the long midi message data to the next sender component in the chain.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <remarks>The method will fail graciously if the <see cref="Successor"/> property is not set.</remarks>
        protected void NextSenderLongData(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, nameof(buffer));

            if (Successor != null)
            {
                Successor.LongData(buffer);
            }
        }

        /// <summary>
        /// Gets or sets the next sender component this instance will call.
        /// </summary>
        public IMidiDataSender Successor { get; set; }
    }
}