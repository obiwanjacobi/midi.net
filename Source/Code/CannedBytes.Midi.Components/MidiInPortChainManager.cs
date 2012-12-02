namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiInPortChainManager manages a chain of <see cref="IMidiReceiver"/> components
    /// that starts with a <see cref="MidiInPort"/>.
    /// </summary>
    public class MidiInPortChainManager : MidiReceiverChainManager<IMidiReceiver>
    {
        /// <summary>
        /// Constructs a new instance for the specified Midi In <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi In Port that represents the source of the chain. Must not be null.</param>
        public MidiInPortChainManager(MidiInPort port)
            : base(port)
        {
            _port = port;
        }

        private MidiInPort _port;

        /// <summary>
        /// Gets the Midi In Port (passed in constructor).
        /// </summary>
        public MidiInPort MidiPort
        {
            get { return _port; }
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
            MidiPort.MidiBufferManager.Initialize(numberOfBuffers, bufferSize);

            InitializeByMidiPort(MidiPort);
        }
    }
}