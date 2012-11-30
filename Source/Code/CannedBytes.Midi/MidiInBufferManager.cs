using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiInBufferManager manages <see cref="MidiBufferStream"/> instances on behalf of
    /// a <see cref="MidiInPort"/> instance.
    /// </summary>
    public class MidiInBufferManager : MidiBufferManager
    {
        /// <summary>
        /// Initializes the buffer manager on the Midi In <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        internal protected MidiInBufferManager(MidiInPort port)
            : base(port, FileAccess.Read)
        { }

        /// <summary>
        /// Returns the <paramref name="buffer"/> to the pool.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>Call this method when the <paramref name="buffer"/> is no longer needed.</remarks>
        public override void Return(MidiBufferStream buffer)
        {
            // do not re-add buffers during a Reset (or Close) that is meant to return all
            // buffers from the MidiInPort to the buffer manager.
            if (!MidiPort.HasStatus(MidiPortStatus.Reset | MidiPortStatus.Closed))
            {
                // returned buffers are added to the midi in port again
                // to make them available for recording sysex.
                AddBufferToPort(buffer);
            }
            else
            {
                base.Return(buffer);
            }
        }

        /// <summary>
        /// Prepares the <paramref name="buffer"/> for the Midi In Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>This method is not intended to be called by client code.</remarks>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            int result = NativeMethods.midiInPrepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }

        /// <summary>
        /// Un-prepares the <paramref name="buffer"/> for the Midi In Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>This method is not intended to be called by client code.</remarks>
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            int result = NativeMethods.midiInUnprepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }

        /// <summary>
        /// Initializes the buffer pool of the buffer manager.
        /// </summary>
        /// <param name="bufferCount">Specify 0 for no buffers.</param>
        /// <param name="bufferSize">Specify greater than 0.</param>
        public override void Initialize(int bufferCount, int bufferSize)
        {
            if (bufferSize <= 0)
            {
                // default buffer size
                bufferSize = 1024 * 4;
            }

            base.Initialize(bufferCount, bufferSize);
        }

        /// <summary>
        /// Gets the Midi In Port.
        /// </summary>
        public new MidiInPort MidiPort
        {
            get { return (MidiInPort)base.MidiPort; }
        }

        /// <summary>
        /// Registers all buffers in the pool with the Midi In Port.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the Midi In Port is not open.</exception>
        internal void RegisterAllBuffers()
        {
            #region Method Checks

            if (!MidiPort.HasStatus(MidiPortStatus.Open))
            {
                throw new InvalidOperationException(Properties.Resources.MidiInBufferManager_PortNotOpen);
            }

            #endregion Method Checks

            // add buffers to port
            for (int n = 0; n < BufferCount; n++)
            {
                MidiBufferStream buffer = Retrieve();
                AddBufferToPort(buffer);
            }
        }

        private void AddBufferToPort(MidiBufferStream buffer)
        {
            int result = NativeMethods.midiInAddBuffer(MidiPort.MidiSafeHandle,
                buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiInPort.ThrowIfError(result);
        }
    }
}