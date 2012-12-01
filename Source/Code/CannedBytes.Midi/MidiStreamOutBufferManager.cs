namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutBufferManager class manages <see cref="MidiBufferStream"/> instances on behalf of the
    /// <see cref="MidiStreamOutPort"/>.
    /// </summary>
    public class MidiStreamOutBufferManager : MidiOutBufferManager
    {
        /// <summary>
        /// Constructs a new instance on the Midi Stream Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        internal protected MidiStreamOutBufferManager(MidiStreamOutPort port)
            : base(port)
        { }

        /// <summary>
        /// Gets the Midi Out Port.
        /// </summary>
        public new MidiStreamOutPort MidiPort
        {
            get { return (MidiStreamOutPort)base.MidiPort; }
        }

        /// <summary>
        /// Called to prepare <paramref name="buffer"/> for the port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>Buffers are also marked as streams.</remarks>
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            base.OnPrepareBuffer(buffer);

            // mark buffers as streams
            buffer.HeaderFlags |= NativeMethods.MHDR_ISSTRM;
        }
    }
}