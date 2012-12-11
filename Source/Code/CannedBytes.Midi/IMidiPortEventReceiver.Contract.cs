namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies code contracts for the <see cref="IMidiPortEventReceiver"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiPortEventReceiver))]
    internal abstract class MidiPortEventReceiverContract : IMidiPortEventReceiver
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="portEvent">Must not be null.</param>
        void IMidiPortEventReceiver.PortEvent(MidiPortEvent portEvent)
        {
            Contract.Requires(portEvent != null);

            throw new NotImplementedException();
        }
    }
}