using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A basic interface to a midi message.
    /// </summary>
    [ContractClass(typeof(MidiMessageContract))]
    public interface IMidiMessage
    {
        /// <summary>
        /// The length of the message in bytes.
        /// </summary>
        int ByteLength { get; }

        /// <summary>
        /// Retrieves the message as a byte array.
        /// </summary>
        /// <returns>Never returns null. Do not modify the returned array.</returns>
        byte[] GetData();
    }

    [ContractClassFor(typeof(IMidiMessage))]
    internal abstract class MidiMessageContract : IMidiMessage
    {
        private MidiMessageContract()
        { }

        int IMidiMessage.ByteLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                throw new System.NotImplementedException();
            }
        }

        byte[] IMidiMessage.GetData()
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);

            throw new System.NotImplementedException();
        }
    }
}