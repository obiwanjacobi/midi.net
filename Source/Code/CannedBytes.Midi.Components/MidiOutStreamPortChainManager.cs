namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiStreamOutPortChainManager manages a chain of <see cref="IMidiDataSender"/> components
    /// that starts with a <see cref="MidiOutStreamPort"/>.
    /// </summary>
    public class MidiOutStreamPortChainManager : MidiSenderChainManager<IMidiDataSender>
    {
        /// <summary>
        /// Constructs a new instance for the specified Midi Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi Stream Out Port that represents the end
        /// of the chain. Must not be null.</param>
        public MidiOutStreamPortChainManager(MidiOutStreamPort port)
            : base(port)
        {
            _port = port;
        }

        private MidiOutStreamPort _port;

        /// <summary>
        /// Gets the Midi Out Port (passed in constructor).
        /// </summary>
        public MidiOutStreamPort MidiPort
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