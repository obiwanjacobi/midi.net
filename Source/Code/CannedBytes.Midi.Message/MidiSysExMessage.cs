namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a System Exclusive midi message.
    /// </summary>
    public class MidiSysExMessage : MidiLongMessage
    {
        /// <summary>
        /// Constructs a new instance on the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Must not be null or empty.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Throw is not recognized.")]
        public MidiSysExMessage(byte[] data)
        {
            Contract.Requires(data != null);
            Contract.Requires(data.Length > 0);
            Throw.IfArgumentNull(data, "data");

            SetData(data);
        }
    }
}