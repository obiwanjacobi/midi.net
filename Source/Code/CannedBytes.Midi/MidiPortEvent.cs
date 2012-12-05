using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CannedBytes.Midi
{
    public class MidiPortEvent
    {
        /// <summary>
        /// Constructs a new instance for a short midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for short midi messages.</param>
        /// <param name="data">The short midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public MidiPortEvent(MidiPortEventTypes recordType, int data, long deltaTime)
        {
            //Throw.IfArgumentOutOfRange<int>((int)recordType,
            //    (int)MidiPortEventTypes.ShortData, (int)MidiPortEventTypes.MoreData, "recordType");

            RecordType = recordType;
            Data = data;
            Buffer = null;
            DeltaTime = deltaTime;
        }

        /// <summary>
        /// Constructs a new instance for a long midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for long midi messages.</param>
        /// <param name="buffer">The long midi message. Must not be null.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public MidiPortEvent(MidiPortEventTypes recordType, MidiBufferStream buffer, long deltaTime)
        {
            //Throw.IfArgumentOutOfRange<int>((int)recordType,
            //    (int)MidiPortEventTypes.LongData, (int)MidiPortEventTypes.LongError, "recordType");

            RecordType = recordType;
            Data = 0;
            Buffer = buffer;
            DeltaTime = deltaTime;
        }

        /// <summary>
        /// Gets the type of record.
        /// </summary>
        public MidiPortEventTypes RecordType { get; private set; }

        /// <summary>
        /// Gets the short midi message.
        /// </summary>
        public int Data { get; private set;}

        /// <summary>
        /// Gets the long midi message.
        /// </summary>
        /// <remarks>Can return null if this record was constructed for a short midi message.</remarks>
        public MidiBufferStream Buffer { get; private set; }

        /// <summary>
        /// Gets A time indication of the midi message.
        /// </summary>
        public long DeltaTime { get; private set;}
    }
}
