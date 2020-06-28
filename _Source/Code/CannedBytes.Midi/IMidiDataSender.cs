namespace CannedBytes.Midi
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The IMidiSender interface is used to communicate midi messages
    /// to be sent up a sender chain.
    /// </summary>
    [ContractClass(typeof(MidiDataSenderContract))]
    public interface IMidiDataSender
    {
        /// <summary>
        /// Sends a short midi message up the sender chain.
        /// </summary>
        /// <param name="data">The short midi message.</param>
        void ShortData(int data);

        /// <summary>
        /// Sends a long midi message up the sender chain.
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        void LongData(MidiBufferStream buffer);
    }
}