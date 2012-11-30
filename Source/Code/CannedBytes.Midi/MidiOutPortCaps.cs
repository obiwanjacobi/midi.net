using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi output port capabilities.
    /// </summary>
    public struct MidiOutPortCaps
    {
        internal MidiOutPortCaps(ref MidiOutCaps caps)
            : this(caps.mid, caps.pid, caps.driverVersion, caps.name,
                caps.technology, caps.voices, caps.notes, caps.channelMask, caps.support)
        { }

        /// <summary>
        /// Constructs an immutable instance.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer Id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="driverVersion">The driver version.</param>
        /// <param name="name">The port name.</param>
        /// <param name="technology">The port technology. <see cref="MidiOutPortCapsTechnology"/></param>
        /// <param name="voices">The number of voices.</param>
        /// <param name="notes">The number of notes.</param>
        /// <param name="channelMask">Supported channels.</param>
        /// <param name="support">Driver support flags. <see cref="MidiOutPortCapsSupport"/></param>
        public MidiOutPortCaps(int manufacturerId, int productId, long driverVersion,
            string name, int technology, int voices, int notes, int channelMask, long support)
        {
            _mid = manufacturerId;
            _pid = productId;
            _driverVersion = driverVersion;
            _name = name;
            _technology = (MidiOutPortCapsTechnology)technology;
            _voices = voices;
            _notes = notes;
            _channelMask = channelMask;
            _support = (MidiOutPortCapsSupport)support;
        }

        private int _mid;

        /// <summary>
        /// Manufacturer identifier of the port driver for the Midi output
        /// port.
        /// </summary>
        public int ManufacturerId
        {
            get { return _mid; }
        }

        private int _pid;

        /// <summary>
        /// Product identifier of the Midi output port.
        /// </summary>
        public int ProductId
        {
            get { return _pid; }
        }

        private long _driverVersion;

        /// <summary>
        /// Version number of the port driver for the Midi output port. The
        /// high-order byte is the major version number, and the low-order byte
        /// is the minor version number.
        /// </summary>
        public long DriverVersion
        {
            get { return _driverVersion; }
        }

        private string _name;

        /// <summary>
        /// Product name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        private MidiOutPortCapsTechnology _technology;

        /// <summary>
        /// EventType describing the type of the Midi output port.
        /// </summary>
        public MidiOutPortCapsTechnology Technology
        {
            get { return _technology; }
        }

        private int _voices;

        /// <summary>
        /// Number of voices supported by an internal synthesizer port. If
        /// the port is an external port, this member is not meaningful
        /// and is set to 0.
        /// </summary>
        public int Voices
        {
            get { return _voices; }
        }

        private int _notes;

        /// <summary>
        /// Maximum number of simultaneous notes that can be played by an
        /// internal synthesizer device. If the device is a port, this
        /// member is not meaningful and is set to 0.
        /// </summary>
        public int Notes
        {
            get { return _notes; }
        }

        private int _channelMask;

        /// <summary>
        /// Channels that an internal synthesizer port responds to, where the
        /// least significant bit refers to channel 0 and the most significant
        /// bit to channel 15. Port ports that transmit on all channels set
        /// this member to 0xFFFF.
        /// </summary>
        public int ChannelMask
        {
            get { return _channelMask; }
        }

        private MidiOutPortCapsSupport _support;

        /// <summary>
        /// Optional functionality supported by the port.
        /// </summary>
        public MidiOutPortCapsSupport Support
        {
            get { return _support; }
        }
    }

    /// <summary>
    /// Represents the Windows Multimedia MIDIOUTCAPS structure.
    /// </summary>
    internal struct MidiOutCaps
    {
        public ushort mid;
        public ushort pid;
        public uint driverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
        public ushort technology;
        public ushort voices;
        public ushort notes;
        public ushort channelMask;
        public uint support;
    }
}