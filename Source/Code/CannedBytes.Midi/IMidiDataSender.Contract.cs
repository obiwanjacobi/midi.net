namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies the code contracts for the <see cref="IMidiDataSender"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiDataSender))]
    internal abstract class MidiDataSenderContract : IMidiDataSender
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="data">No contract.</param>
        void IMidiDataSender.ShortData(int data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        void IMidiDataSender.LongData(MidiBufferStream buffer)
        {
            Contract.Requires(buffer != null);

            throw new NotImplementedException();
        }
    }
}