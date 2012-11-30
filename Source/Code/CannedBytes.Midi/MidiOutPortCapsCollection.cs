using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutPortCapsCollection class provides a collection of all available
    /// Midi Out Ports.
    /// </summary>
    /// <remarks>The collection contains instances of the <see cref="MidiOutPortCaps"/> struct.
    /// Instantiating more than one instance is useless.</remarks>
    public class MidiOutPortCapsCollection : ReadOnlyCollection<MidiOutPortCaps>
    {
        /// <summary>
        /// Initializes the collection instance.
        /// </summary>
        public MidiOutPortCapsCollection()
            : base(new List<MidiOutPortCaps>())
        {
            int count = NativeMethods.midiOutGetNumDevs();

            for (int portId = 0; portId < count; portId++)
            {
                MidiOutPortCaps caps = MidiOutPort.GetPortCapabilities(portId);

                base.Items.Add(caps);
            }
        }

        /// <summary>
        /// Returns the capabilities in an array.
        /// </summary>
        /// <returns>Never returns null.</returns>
        public MidiOutPortCaps[] ToArray()
        {
            List<MidiOutPortCaps> caps = new List<MidiOutPortCaps>(Items);

            return caps.ToArray();
        }
    }
}