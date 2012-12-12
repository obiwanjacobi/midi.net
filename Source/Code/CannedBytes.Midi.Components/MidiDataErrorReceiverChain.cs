namespace CannedBytes.Midi.Components
{
    using System.Diagnostics.Contracts;

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
        /// <param name="timeIndex">The time at which the message was received.</param>
        protected void NextReceiverShortError(int data, int timeIndex)
        {
            if (this.Next != null)
            {
                this.Next.ShortError(data, timeIndex);
            }
        }

        /// <summary>
        /// Calls the <see cref="M:IMidiErrorReceiver.LongError"/> on the
        /// <see cref="P:NextReceiver"/> instance (if not null).
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <param name="timeIndex">The time at which the message was received.</param>
        protected void NextReceiverLongError(MidiBufferStream buffer, int timeIndex)
        {
            Contract.Requires(buffer != null);
            Throw.IfArgumentNull(buffer, "buffer");

            if (this.Next != null)
            {
                this.Next.LongError(buffer, timeIndex);
            }
        }

        /// <summary>
        /// Gets or sets the next error receiver component in the chain.
        /// </summary>
        /// <remarks>If this value is null (Nothing in VB) it marks the end of the chain.</remarks>
        public IMidiDataErrorReceiver Next { get; set; }
    }
}