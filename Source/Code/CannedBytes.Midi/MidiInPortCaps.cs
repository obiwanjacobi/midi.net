using System.Runtime.InteropServices;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi Input Port capabilities.
    /// </summary>
    public class MidiInPortCaps
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
            ManufacturerId = manufacturerId;
            ProductId = productId;
            DriverVersion = driverVersion;
            Name = name;
            Support = support;
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
        /// Optional functionality supported by the port.
        /// </summary>
        public long Support { get; private set; }
    }

    /// <summary>
    /// Represents the Windows Multimedia MIDIINCAPS structure.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [StructLayout(LayoutKind.Sequential)]
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