using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The IMidiPortEventReceiver is used to pass <see cref="MidiPortEvent"/>s down a receive chain.
    /// </summary>
    /// <remarks>There is no sender interface for port events because only the <see cref="MidiInPort"/> can produce these events.</remarks>
    public interface IMidiPortEventReceiver
    {
        /// <summary>
        /// Passes the received <paramref name="portEvent"/> to the next component in the receive chain.
        /// </summary>
        /// <param name="portEvent">Must not be null.</param>
        void PortEvent(MidiPortEvent portEvent);
    }
}
