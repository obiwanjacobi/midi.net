namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiErrorReceiverChain class implements an abstract base class for error
    /// receiver chain components.
    /// </summary>
    /// <remarks>Derived classes should implement the <see cref="IMidiDataErrorReceiver"/>
    /// interface and use the <see cref="M:NextReceieverShortError"/> and
    /// <see cref="NextReceiverLongError"/> methods from inside the implemented interface
    /// methods to call the next component in the chain.</remarks>
    public abstract class MidiDataErrorReceiverChain : IChainOf<IMidiDataErrorReceiver>
    {
        /// <summary>
        /// Calls the <see cref="M:IMidiErrorReceiver.ShortError"/> on the
        /// <see cref="P:NextReceiver"/> instance (if not null).
        /// </summary>
        /// <param name="data">The short midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        protected void NextReceiverShortError(int data, long timestamp)
        {
            if (Successor != null)
            {
                Successor.ShortError(data, timestamp);
            }
        }

        /// <summary>
        /// Calls the <see cref="M:IMidiErrorReceiver.LongError"/> on the
        /// <see cref="P:NextReceiver"/> instance (if not null).
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <param name="timestamp">The time at which the message was received.</param>
        protected void NextReceiverLongError(MidiBufferStream buffer, long timestamp)
        {
            Check.IfArgumentNull(buffer, nameof(buffer));

            if (Successor != null)
            {
                Successor.LongError(buffer, timestamp);
            }
        }

        /// <summary>
        /// Gets or sets the next error receiver component in the chain.
        /// </summary>
        /// <remarks>If this value is null (Nothing in VB) it marks the end of the chain.</remarks>
        public IMidiDataErrorReceiver Successor { get; set; }
    }
}