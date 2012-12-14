namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies the code contract for the <see cref="IMidiDataReceiver"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiDataReceiver))]
    internal abstract class MidiDataReceiverContract : IMidiDataReceiver
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="data">No contract.</param>
        /// <param name="timestamp">No contract.</param>
        void IMidiDataReceiver.ShortData(int data, long timestamp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <param name="timestamp">No contract.</param>
        void IMidiDataReceiver.LongData(MidiBufferStream buffer, long timestamp)
        {
            Contract.Requires(buffer != null);

            throw new NotImplementedException();
        }
    }
}