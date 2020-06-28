namespace CannedBytes.Midi
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The MidiOutBufferManager class manages <see cref="MidiBufferStream"/> instances on behalf of the
    /// <see cref="MidiOutStreamPort"/>.
    /// </summary>
    public class MidiOutStreamBufferManager : MidiOutBufferManager
    {
        /// <summary>
        /// Constructs a new instance on the Midi Stream Out <paramref name="port"/>.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        protected internal MidiOutStreamBufferManager(MidiOutStreamPort port)
            : base(port)
        {
        }

        /// <summary>
        /// Gets the Midi Out Port.
        /// </summary>
        public new MidiOutStreamPort MidiPort
        {
            get { return (MidiOutStreamPort)base.MidiPort; }
        }

        /// <summary>
        /// Called to prepare <paramref name="buffer"/> for the port.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <remarks>Buffers are also marked as streams.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        protected override void OnPrepareBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            base.OnPrepareBuffer(buffer);

            // mark buffers as streams
            buffer.HeaderFlags |= NativeMethods.MHDR_ISSTRM;
        }
    }
}