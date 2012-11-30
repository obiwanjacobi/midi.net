namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutBufferManager class manages <see cref="MidiBufferStream"/> instances on behalf of the
    /// <see cref="MidiStreamOutPort"/>.
    /// </summary>
    public class MidiStreamOutBufferManager : MidiOutBufferManagerBase
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
        /// Creates a new <see cref="MidiBufferStream"/> instance.
        /// </summary>
        /// <returns>Never returns null.</returns>
        /// <remarks>The buffer is marked as a stream (MHDR_ISSTRM).</remarks>
        //protected override MidiBufferStream CreateMidiBuffer()
        //{
        //    MidiBufferStream buffer = base.CreateMidiBuffer();

        //    // mark buffers as streams
        //    buffer.MidiHeader.flags |= NativeMethods.MHDR_ISSTRM;

        //    return buffer;
        //}

        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            base.OnPrepareBuffer(buffer);

            // mark buffers as streams
            buffer.HeaderFlags |= NativeMethods.MHDR_ISSTRM;
        }
    }
}