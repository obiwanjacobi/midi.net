namespace CannedBytes.Midi
{
    /// <summary>
    /// The IMidiSender interface is used to communicate midi messages
    /// to be sent up a sender chain.
    /// </summary>
    public interface IMidiSender
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

    /// <summary>
    /// To be used as a completed or notify callback.
    /// </summary>
    /// <param name="port"></param>
    /// <param name="e"></param>
    public delegate void MidiPortCallback(IMidiPort port, MidiPortSenderEventArgs e);

    /// <summary>
    /// The next version of the sender interface.
    /// </summary>
    public interface IMidiSenderWithCallback
    {
        /// <summary>
        /// Under construction...
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        void LongData(MidiBufferStream buffer, MidiPortCallback callback);
    }
}