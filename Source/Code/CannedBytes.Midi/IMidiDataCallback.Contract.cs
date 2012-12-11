namespace CannedBytes.Midi
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Specifies the code contract for the <see cref="IMidiDataCallback"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiDataCallback))]
    internal abstract class MidiDataCallbackContract : IMidiDataCallback
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <param name="notificationType">No contract.</param>
        void IMidiDataCallback.LongData(MidiBufferStream buffer, MidiDataCallbackType notificationType)
        {
            Contract.Requires(buffer != null);

            throw new System.NotImplementedException();
        }
    }
}