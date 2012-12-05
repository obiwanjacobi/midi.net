namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiErrorReceiverChain class implements an abstract base class for error
    /// receiver chain components.
    /// </summary>
    /// <remarks>Derived classes should implement the <see cref="IMidiErrorReceiver"/>
    /// interface and use the <see cref="M:NextReceieverShortError"/> and
    /// <see cref="NextReceiverLongError"/> methods from inside the implemented interface
    /// methods to call the next component in the chain.</remarks>
    public abstract class MidiErrorReceiverChain : IChainOf<IMidiDataErrorReceiver>
    {
        /// <summary>
        /// Calls the <see cref="M:IMidiErrorReceiver.ShortError"/> on the
        /// <see cref="P:NextReceiver"/> instance (if not null).
        /// </summary>
        /// <param name="data">The short midi message.</param>
        /// <param name="timeIndex">The time at which the message was received.</param>
        protected void NextReceiverShortError(int data, int timeIndex)
        {
            if (Next != null)
            {
                Next.ShortError(data, timeIndex);
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
            if (Next != null)
            {
                Next.LongError(buffer, timeIndex);
            }
        }

        private IMidiDataErrorReceiver _errorReceiver;

        /// <summary>
        /// Gets or sets the next error receiver component in the chain.
        /// </summary>
        /// <remarks>If this value is null (Nothing in VB) it marks the end of the chain.</remarks>
        public IMidiDataErrorReceiver Next
        {
            get { return _errorReceiver; }
            set { _errorReceiver = value; }
        }
    }
}