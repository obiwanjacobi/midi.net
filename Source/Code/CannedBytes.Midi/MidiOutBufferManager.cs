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
        { }

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
            //Contract.Requires<ArgumentNullException>(buffer != null);

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
            //Contract.Requires<ArgumentNullException>(buffer != null);

            int result = NativeMethods.midiOutUnprepareHeader(
                MidiPort.MidiSafeHandle, buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            MidiOutPortBase.ThrowIfError(result);
        }
    }
}