using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiInPortCapsCollection class provides a collection of all available
    /// Midi In Ports.
    /// </summary>
    /// <remarks>The collection contains instances of the <see cref="MidiInPortCaps"/> struct.
    /// Instantiating more than one instance is useless.</remarks>
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

                base.Items.Add(caps);
            }
        }

        /// <summary>
        /// Returns the capabilities in an array.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public MidiInPortCaps[] ToArray()
        {
            List<MidiInPortCaps> caps = new List<MidiInPortCaps>(Items);

            return caps.ToArray();
        }
    }
}