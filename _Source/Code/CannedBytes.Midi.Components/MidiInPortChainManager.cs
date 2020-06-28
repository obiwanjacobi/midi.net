namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiInPortChainManager manages a chain of <see cref="IMidiDataReceiver"/> components
    /// that starts with a <see cref="MidiInPort"/>.
    /// </summary>
    public class MidiInPortChainManager : MidiReceiverChainManager<IMidiDataReceiver, MidiInPort>
    {
        /// <summary>
        /// Constructs a new instance for the specified Midi In <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi In Port that represents the source of the chain. Must not be null.</param>
        public MidiInPortChainManager(MidiInPort port)
            : base(port)
        {
            Check.IfArgumentNull(port, "port");
        }

        /// <summary>
        /// Initializes the <see cref="MidiInBufferManager"/> and all the components in the
        /// chain that implement the <see cref="T:IInitializeByMidiPort"/> interface.
        /// </summary>
        /// <param name="numberOfBuffers">The number of buffer to use for receiving SysEx messages.
        /// Pass zero to use default.</param>
        /// <param name="bufferSize">The size of each buffer in the pool. Pass zero to use default.</param>
        public void Initialize(int numberOfBuffers, int bufferSize)
        {
            // initialize the buffer manager for the in-port
            MidiPort.BufferManager.Initialize(numberOfBuffers, bufferSize);

            this.Initialize();
        }
    }
}