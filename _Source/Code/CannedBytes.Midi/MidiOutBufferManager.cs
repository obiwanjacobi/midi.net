namespace CannedBytes.Midi
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// The MidiOutBufferManagerBase class provides a base implementation
    /// of a <see cref="MidiBufferManager"/> for a Midi Out Port.
    /// </summary>
    public class MidiOutBufferManager : MidiBufferManager
    {
        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">A midi port base class.</param>
        internal MidiOutBufferManager(MidiOutPortBase port)
            : base(port, FileAccess.ReadWrite)
        {
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
                PrepareAllBuffers();
            }
        }

        /// <summary>
        /// Prepares a <paramref name="buffer"/> to be passed to the Midi Out Port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiOutPrepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }

        /// <summary>
        /// Un-prepares a <paramref name="buffer"/> that was finished.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        protected override void OnUnprepareBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            int result = NativeMethods.midiOutUnprepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }
    }
}