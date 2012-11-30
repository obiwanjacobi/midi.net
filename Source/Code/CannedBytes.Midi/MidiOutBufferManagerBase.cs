using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutBufferManagerBase class provides a base implementation
    /// of a <see cref="MidiBufferManager"/> for a Midi Out Port.
    /// </summary>
    public abstract class MidiOutBufferManagerBase : MidiBufferManager
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">A midi port base class</param>
        protected MidiOutBufferManagerBase(MidiPort port)
            : base(port, FileAccess.ReadWrite)
        { }

        /// <summary>
        /// Prepares a <paramref name="buffer"/> to be passed to the Midi Out Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            int result = NativeMethods.midiOutPrepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }

        /// <summary>
        /// Un-prepares a <paramref name="buffer"/> that was finished.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>This method is not intended to be called by client code.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            int result = NativeMethods.midiOutUnprepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }

        /// <summary>
        /// Gets the Midi Port.
        /// </summary>
        public new MidiOutPortBase MidiPort
        {
            get { return (MidiOutPortBase)base.MidiPort; }
        }
    }
}