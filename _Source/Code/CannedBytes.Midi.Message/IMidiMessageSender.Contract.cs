namespace CannedBytes.Midi.Message
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract specification for the <see cref="IMidiMessageSender"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiMessageSender))]
    internal abstract class MidiMessageSenderContract : IMidiMessageSender
    {
        /// <summary>
        /// Contract specification.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        void IMidiMessageSender.Send(MidiShortMessage message)
        {
            Contract.Requires(message != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract specification.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        void IMidiMessageSender.Send(MidiLongMessage message)
        {
            Contract.Requires(message != null);

            throw new NotImplementedException();
        }
    }
}