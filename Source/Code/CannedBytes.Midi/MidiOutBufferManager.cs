using System.Diagnostics.Contracts;
using System.IO;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutBufferManagerBase class provides a base implementation
    /// of a <see cref="MidiBufferManager"/> for a Midi Out Port.
    /// </summary>
    public class MidiOutBufferManager : MidiBufferManager
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">A midi port base class</param>
        internal MidiOutBufferManager(MidiOutPortBase port)
            : base(port, FileAccess.ReadWrite)
        {
            Contract.Requires(port != null);
        }

        /// <summary>
        /// Initializes the buffers this instance manages.
        /// </summary>
        /// <param name="bufferCount">The number of buffers.</param>
        /// <param name="bufferSize">The size in bytes of each buffer.</param>
        public override void Initialize(int bufferCount, int bufferSize)
        {
            base.Initialize(bufferCount, bufferSize);

            if (MidiPort.IsOpen)
            {
                base.PrepareAllBuffers();
            }
        }

        /// <summary>
        /// Prepares a <paramref name="buffer"/> to be passed to the Midi Out Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiOutPrepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }

        /// <summary>
        /// Un-prepares a <paramref name="buffer"/> that was finished.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiOutUnprepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }
    }
}