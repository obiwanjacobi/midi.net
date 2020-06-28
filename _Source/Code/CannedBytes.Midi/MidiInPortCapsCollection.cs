namespace CannedBytes.Midi
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The MidiInPortCapsCollection class provides a collection of all available
    /// Midi In Ports.
    /// </summary>
    /// <remarks>The collection contains instances of the <see cref="MidiInPortCaps"/> class.
    /// Instantiating more than one instance is useless. The instance is populated in its ctor.</remarks>
    public class MidiInPortCapsCollection : ReadOnlyCollection<MidiInPortCaps>
    {
        /// <summary>
        /// Initializes the collection instance.
        /// </summary>
        public MidiInPortCapsCollection()
            : base(new List<MidiInPortCaps>())
        {
            int count = NativeMethods.midiInGetNumDevs();

            for (int portId = 0; portId < count; portId++)
            {
                MidiInPortCaps caps = MidiInPort.GetPortCapabilities(portId);

                Items.Add(caps);
            }
        }
    }
}