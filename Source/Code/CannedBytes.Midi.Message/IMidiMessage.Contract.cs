namespace CannedBytes.Midi.Message
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contract specification for the <see cref="IMidiMessage"/> interface.
    /// </summary>
    [ContractClassFor(typeof(IMidiMessage))]
    internal abstract class MidiMessageContract : IMidiMessage
    {
        /// <summary>
        /// Contract ensures return value is greater than zero.
        /// </summary>
        int IMidiMessage.ByteLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);

                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Contract specification.
        /// </summary>
        /// <returns>Returns a non-null value.</returns>
        byte[] IMidiMessage.GetData()
        {
            Contract.Ensures(Contract.Result<byte[]>() != null);

            throw new System.NotImplementedException();
        }
    }
}