namespace CannedBytes.Midi
{
    /// <summary>
    /// Represents Midi Input Port capabilities.
    /// </summary>
    public class MidiInPortCaps
    {
        /// <summary>
        /// Constructs a new instance based on the unmanaged structure.
        /// </summary>
        /// <param name="caps">Reference to the unmanaged structure.</param>
        internal MidiInPortCaps(ref MidiInCaps caps)
            : this(caps.Mid, caps.Pid, caps.DriverVersion, caps.Name, caps.Support)
        {
        }

        /// <summary>
        /// Constructs an immutable instance.
        /// </summary>
        /// <param name="manufacturerId">The manufacturer Id.</param>
        /// <param name="productId">The product Id.</param>
        /// <param name="driverVersion">The driver version.</param>
        /// <param name="name">The port name.</param>
        /// <param name="support">Driver support flags.</param>
        public MidiInPortCaps(int manufacturerId, int productId, long driverVersion, string name, long support)
        {
            this.ManufacturerId = manufacturerId;
            this.ProductId = productId;
            this.DriverVersion = driverVersion;
            this.Name = name;
            this.Support = support;
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
}