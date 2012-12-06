using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi output port capabilities.
    /// </summary>
    public class MidiOutPortCaps
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
            ManufacturerId = manufacturerId;
            ProductId = productId;
            DriverVersion = driverVersion;
            Name = name;
            Technology = (MidiOutPortCapsTechnology)technology;
            Voices = voices;
            Notes = notes;
            ChannelMask = channelMask;
            Support = (MidiOutPortCapsSupport)support;
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
        public MidiOutPortCapsSupport Support { get; private set; }
    }

    /// <summary>
    /// Represents the Windows Multimedia MIDIOUTCAPS structure.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [StructLayout(LayoutKind.Sequential)]
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