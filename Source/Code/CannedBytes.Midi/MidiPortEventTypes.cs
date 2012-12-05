using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CannedBytes.Midi
{
    public enum MidiPortEventTypes
    {
        /// <summary>An invalid value.</summary>
        None,
        /// <summary>Record contains a short midi message.</summary>
        ShortData,
        /// <summary>Record contains a short midi message error.</summary>
        ShortError,
        /// <summary>Record contains another short midi message.</summary>
        MoreData,
        /// <summary>Record contains a long midi message.</summary>
        LongData,
        /// <summary>Record contains a long midi message error.</summary>
        LongError,
    }
}
