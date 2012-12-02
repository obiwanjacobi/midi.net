namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiStreamOutPortChainManager manages a chain of <see cref="IMidiSender"/> components
    /// that starts with a <see cref="MidiStreamOutPort"/>.
    /// </summary>
    public class MidiStreamOutPortChainManager : MidiSenderChainManager<IMidiSender>
    {
        /// <summary>
        /// Constructs a new instance for the specified Midi Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi Stream Out Port that represents the end
        /// of the chain. Must not be null.</param>
        public MidiStreamOutPortChainManager(MidiStreamOutPort port)
            : base(port)
        {
            _port = port;
        }

        private MidiStreamOutPort _port;

        /// <summary>
        /// Gets the Midi Out Port (passed in constructor).
        /// </summary>
        public MidiStreamOutPort MidiPort
        {
            get { return _port; }
        }

        /// <summary>
        /// Initializes all the components in the chain that implement the
        /// <see cref="T:IInitializeByMidiPort"/> interface.
        /// </summary>
        public void Initialize()
        {
            InitializeByMidiPort(MidiPort);
        }
    }
}