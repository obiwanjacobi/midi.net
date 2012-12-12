namespace CannedBytes.Midi
{
    /// <summary>
    /// Information on a Midi Event source from a Midi (In) Port.
    /// </summary>
    public class MidiPortEvent
    {
        /// <summary>
        /// Constructs a new instance for a short midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for short midi messages.</param>
        /// <param name="data">The short midi message.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public MidiPortEvent(MidiPortEventType recordType, int data, long deltaTime)
        {
            Throw.IfArgumentOutOfRange<int>((int)recordType, (int)MidiPortEventType.ShortData, (int)MidiPortEventType.MoreData, "recordType");

            this.RecordType = recordType;
            this.Data = data;
            this.Buffer = null;
            this.DeltaTime = deltaTime;
        }

        /// <summary>
        /// Constructs a new instance for a long midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for long midi messages.</param>
        /// <param name="buffer">The long midi message. Must not be null.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public MidiPortEvent(MidiPortEventType recordType, MidiBufferStream buffer, long deltaTime)
        {
            Throw.IfArgumentOutOfRange<int>((int)recordType, (int)MidiPortEventType.LongData, (int)MidiPortEventType.LongError, "recordType");

            this.RecordType = recordType;
            this.Data = 0;
            this.Buffer = buffer;
            this.DeltaTime = deltaTime;
        }

        /// <summary>
        /// Gets the type of record.
        /// </summary>
        public MidiPortEventType RecordType { get; private set; }

        /// <summary>
        /// Gets the short midi message.
        /// </summary>
        public int Data { get; private set; }

        /// <summary>
        /// Gets the long midi message.
        /// </summary>
        /// <remarks>Can return null if this record was constructed for a short midi message.</remarks>
        public MidiBufferStream Buffer { get; private set; }

        /// <summary>
        /// Gets A time indication of the midi message.
        /// </summary>
        public long DeltaTime { get; private set; }
    }
}