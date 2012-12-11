namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>
    /// Abstract class template for contracts for abstract <see cref="MidiBufferManager"/> class.
    /// </summary>
    [ContractClassFor(typeof(MidiBufferManager))]
    internal abstract class MidiBufferManagerContract : MidiBufferManager
    {
        /// <summary>
        /// Hidden.
        /// </summary>
        private MidiBufferManagerContract()
            : base(null, FileAccess.Read)
        {
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires(buffer != null);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Contract.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires(buffer != null);

            throw new NotImplementedException();
        }
    }
}