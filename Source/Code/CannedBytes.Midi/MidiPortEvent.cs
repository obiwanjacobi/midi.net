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
            Check.IfArgumentOutOfRange<int>((int)recordType, (int)MidiPortEventType.ShortData, (int)MidiPortEventType.MoreData, nameof(recordType));

            RecordType = recordType;
            Data = data;
            Stream = null;
            Timestamp = deltaTime;
        }

        /// <summary>
        /// Constructs a new instance for a long midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for long midi messages.</param>
        /// <param name="stream">The long midi message. Must not be null.</param>
        /// <param name="deltaTime">A time indication of the midi message.</param>
        public MidiPortEvent(MidiPortEventType recordType, IMidiStream stream, long deltaTime)
        {
            Check.IfArgumentOutOfRange<int>((int)recordType, (int)MidiPortEventType.LongData, (int)MidiPortEventType.LongError, nameof(recordType));

            RecordType = recordType;
            Data = 0;
            Stream = stream;
            Timestamp = deltaTime;
        }

        /// <summary>
        /// Gets an indication if the port is event is a short message, otherwise a long message.
        /// </summary>
        public bool IsShortMessage
        {
            get { return RecordType >= MidiPortEventType.ShortData && RecordType <= MidiPortEventType.MoreData; }
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
        public IMidiStream Stream { get; private set; }

        /// <summary>
        /// Gets A time indication of the midi message.
        /// </summary>
        public long Timestamp { get; private set; }
    }
}