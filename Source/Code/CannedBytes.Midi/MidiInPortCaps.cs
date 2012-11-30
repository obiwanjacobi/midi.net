using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi Input Port capabilities.
    /// </summary>
    public struct MidiInPortCaps
    {
        internal MidiInPortCaps(ref MidiInCaps caps)
            : this(caps.mid, caps.pid, caps.driverVersion, caps.name, caps.support)
        { }

        /// <summary>
        /// Constructs an immutable instance.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer Id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="driverVersion">The driver version.</param>
        /// <param name="name">The port name.</param>
        /// <param name="support">Driver support flags.</param>
        public MidiInPortCaps(int manufacturerId, int productId,
            long driverVersion, string name, long support)
        {
            _mid = manufacturerId;
            _pid = productId;
            _driverVersion = driverVersion;
            _name = name;
            _support = support;
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

        private long _support;

        /// <summary>
        /// Optional functionality supported by the port.
        /// </summary>
        public long Support
        {
            get { return _support; }
        }
    }

    /// <summary>
    /// Represents the Windows Multimedia MIDIINCAPS structure.
    /// </summary>
    internal struct MidiInCaps
    {
        public ushort mid;
        public ushort pid;
        public uint driverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
        public uint support;
    }
}