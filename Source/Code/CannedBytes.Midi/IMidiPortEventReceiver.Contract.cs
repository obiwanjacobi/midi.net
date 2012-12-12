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
        /// <param name="midiEvent">Must not be null.</param>
        void IMidiPortEventReceiver.PortEvent(MidiPortEvent midiEvent)
        {
            Contract.Requires(midiEvent != null);

            throw new NotImplementedException();
        }
    }
}