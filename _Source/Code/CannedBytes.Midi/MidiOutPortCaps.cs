namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi output port capabilities.
    /// </summary>
    public class MidiOutPortCaps
    {
        /// <summary>
        /// Constructs a new instance based on the unmanaged structure.
        /// </summary>
        /// <param name="caps">Reference to the unmanaged structure.</param>
        internal MidiOutPortCaps(ref MidiOutCaps caps)
            : this(caps.Mid, caps.Pid, caps.DriverVersion, caps.Name, caps.Technology, caps.Voices, caps.Notes, caps.ChannelMask, caps.Support)
        {
        }

        /// <summary>
        /// Constructs an immutable instance.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer Id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="driverVersion">The driver version.</param>
        /// <param name="name">The port name.</param>
        /// <param name="technology">The port technology: <see cref="MidiOutPortCapsTechnology"/>.</param>
        /// <param name="voices">The number of voices.</param>
        /// <param name="notes">The number of notes.</param>
        /// <param name="channelMask">Supported channels.</param>
        /// <param name="support">Driver support flags: <see cref="MidiOutPortCapsSupportTypes"/>.</param>
        public MidiOutPortCaps(
               int manufacturerId,
               int productId,
               long driverVersion,
               string name,
               int technology,
               int voices,
               int notes,
               int channelMask,
               long support)
        {
            this.ManufacturerId = manufacturerId;
            this.ProductId = productId;
            this.DriverVersion = driverVersion;
            this.Name = name;
            this.Technology = (MidiOutPortCapsTechnology)technology;
            this.Voices = voices;
            this.Notes = notes;
            this.ChannelMask = channelMask;
            this.Support = (MidiOutPortCapsSupportTypes)support;
        }

        /// <summary>
        /// Manufacturer identifier of the port driver for the Midi output
        /// port.
        /// </summary>
        public int ManufacturerId { get; private set; }

        /// <summary>
        /// Product identifier of the Midi output port.
        /// </summary>
        public int ProductId { get; private set; }

        /// <summary>
        /// Version number of the port driver for the Midi output port. The
        /// high-order byte is the major version number, and the low-order byte
        /// is the minor version number.
        /// </summary>
        public long DriverVersion { get; private set; }

        /// <summary>
        /// Product name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// EventType describing the type of the Midi output port.
        /// </summary>
        public MidiOutPortCapsTechnology Technology { get; private set; }

        /// <summary>
        /// Number of voices supported by an internal synthesizer port. If
        /// the port is an external port, this member is not meaningful
        /// and is set to 0.
        /// </summary>
        public int Voices { get; private set; }

        /// <summary>
        /// Maximum number of simultaneous notes that can be played by an
        /// internal synthesizer device. If the device is a port, this
        /// member is not meaningful and is set to 0.
        /// </summary>
        public int Notes { get; private set; }

        /// <summary>
        /// Channels that an internal synthesizer port responds to, where the
        /// least significant bit refers to channel 0 and the most significant
        /// bit to channel 15. Port ports that transmit on all channels set
        /// this member to 0xFFFF.
        /// </summary>
        public int ChannelMask { get; private set; }

        /// <summary>
        /// Optional functionality supported by the port.
        /// </summary>
        public MidiOutPortCapsSupportTypes Support { get; private set; }
    }
}