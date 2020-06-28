namespace CannedBytes.Midi
{
    using System;

    /// <summary>
    /// The IMidiPort interface is implemented by the midi ports.
    /// It provides access to the shared properties and methods common
    /// to all midi port implementations.
    /// </summary>
    public interface IMidiPort
    {
        /// <summary>
        /// Gets a status value for the midi port.
        /// </summary>
        MidiPortStatus Status { get; }

        /// <summary>
        /// Gets the port id; an integer representing a zero-based index.
        /// </summary>
        /// <remarks>Port id's are unique for midi in- and midi out ports. </remarks>
        int PortId { get; }

        /// <summary>
        /// Gets or sets a value that indicates if <see cref="MidiBufferStream"/>'s
        /// should be returned to the <see cref="MidiBufferManager"/> automatically.
        /// </summary>
        bool AutoReturnBuffers { get; set; }

        /// <summary>
        /// Opens the midi port represent by the specified <paramref name="deviceId"/>.
        /// </summary>
        /// <param name="deviceId">A zero-based index port identifier.</param>
        void Open(int deviceId);

        /// <summary>
        /// Closes the midi port.
        /// </summary>
        void Close();

        /// <summary>
        /// Resets the midi port.
        /// </summary>
        /// <remarks>Will return buffers to the manager.</remarks>
        void Reset();

        /// <summary>
        /// Queries the <see cref="Status"/> property of the midi port
        /// if the specified flag is present.
        /// </summary>
        /// <param name="portStatus">The status value to test.</param>
        /// <returns>Returns true if the specified <paramref name="portStatus"/> is
        /// present on the midi port <see cref="Status"/> otherwise false is returned.</returns>
        bool HasStatus(MidiPortStatus portStatus);

        /// <summary>
        /// The StatusChanged event fires after the midi port has changed status.
        /// </summary>
        event EventHandler StatusChanged;
    }
}