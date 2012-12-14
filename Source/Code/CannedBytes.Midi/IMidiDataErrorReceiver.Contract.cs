namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies a code contract for the <see cref="IMidiDataErrorReceiver"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiDataErrorReceiver))]
    internal abstract class MidiDataErrorReceiverContract : IMidiDataErrorReceiver
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="data">No contract.</param>
        /// <param name="timestamp">No contract.</param>
        void IMidiDataErrorReceiver.ShortError(int data, long timestamp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <param name="timestamp">No contract.</param>
        void IMidiDataErrorReceiver.LongError(MidiBufferStream buffer, long timestamp)
        {
            Contract.Requires(buffer != null);

            throw new NotImplementedException();
        }
    }
}