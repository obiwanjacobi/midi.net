namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutBufferManager class manages <see cref="MidiBufferStream"/> instances on behalf of the
    /// <see cref="MidiOutPort"/>.
    /// </summary>
    public class MidiOutBufferManager : MidiOutBufferManagerBase
    {
        /// <summary>
        /// Constructs a new instance on the Midi Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        internal protected MidiOutBufferManager(MidiOutPort port)
            : base(port)
        { }

        /// <summary>
        /// Gets the Midi Out Port.
        /// </summary>
        public new MidiOutPort MidiPort
        {
            get { return (MidiOutPort)base.MidiPort; }
        }
    }
}