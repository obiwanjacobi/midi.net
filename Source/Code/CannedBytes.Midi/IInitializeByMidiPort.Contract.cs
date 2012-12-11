namespace CannedBytes.Midi
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A template class for specifying contracts for the interface.
    /// </summary>
    [ContractClassFor(typeof(IInitializeByMidiPort))]
    internal abstract class InitializeByMidiPortContract : IInitializeByMidiPort
    {
        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        void IInitializeByMidiPort.Initialize(IMidiPort port)
        {
            Contract.Requires(port != null);

            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        void IInitializeByMidiPort.Uninitialize(IMidiPort port)
        {
            Contract.Requires(port != null);

            throw new System.NotImplementedException();
        }
    }
}