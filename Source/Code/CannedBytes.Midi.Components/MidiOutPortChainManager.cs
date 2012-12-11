using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiOutPortChainManager manages a chain of <see cref="IMidiDataSender"/> components
    /// that starts with a <see cref="MidiOutPort"/>.
    /// </summary>
    public class MidiOutPortChainManager : MidiSenderChainManager<IMidiDataSender>
    {
        /// <summary>
        /// Constructs a new instance for the specified Midi Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">The Midi Out Port that represents the end of the chain. Must not be null.</param>
        public MidiOutPortChainManager(MidiOutPort port)
            : base(port)
        {
            Contract.Requires(port != null);
            Throw.IfArgumentNull(port, "port");

            MidiPort = port;
        }

        /// <summary>
        /// Gets the Midi Out Port (passed in constructor).
        /// </summary>
        public new MidiOutPort MidiPort { get; private set; }

        /// <summary>
        /// Initializes the <see cref="MidiOutBufferManager"/> and all the components in the
        /// chain that implement the <see cref="T:IInitializeByMidiPort"/> interface.
        /// </summary>
        /// <param name="bufferCount">The number of buffers to create.</param>
        /// <param name="bufferSize">The size in bytes of each buffer.</param>
        public void Initialize(int bufferCount, int bufferSize)
        {
            // initialize the buffer manager for the out-port
            MidiPort.BufferManager.Initialize(bufferCount, bufferSize);

            InitializeByMidiPort(MidiPort);
        }
    }
}