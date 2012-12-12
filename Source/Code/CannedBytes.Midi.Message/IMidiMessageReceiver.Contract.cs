namespace CannedBytes.Midi.Message
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract specification for the <see cref="IMidiMessageReceiver"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiMessageReceiver))]
    internal abstract class MidiMessageReceiverContract : IMidiMessageReceiver
    {
        /// <summary>
        /// Contract specification.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        void IMidiMessageReceiver.ShortMessage(MidiShortMessage message)
        {
            Contract.Requires(message != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract specification.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        void IMidiMessageReceiver.LongMessage(MidiLongMessage message)
        {
            Contract.Requires(message != null);

            throw new NotImplementedException();
        }
    }
}