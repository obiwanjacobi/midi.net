namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiQueueRecord contains the midi message data for a short or long midi message.
    /// </summary>
    /// <remarks>After construction instances are immutable.</remarks>
    public class MidiQueueRecord
    {
        /// <summary>
        /// Constructs a new instance for a short midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for short midi messages.</param>
        /// <param name="data">The short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public MidiQueueRecord(MidiQueueRecordType recordType, int data, int timeIndex)
        {
            //Throw.IfArgumentOutOfRange<int>((int)recordType,
            //    (int)MidiQueueRecordType.ShortData, (int)MidiQueueRecordType.MoreData, "recordType");

            _type = recordType;
            _data = data;
            _buffer = null;
            _timeIndex = timeIndex;
        }

        /// <summary>
        /// Constructs a new instance for a long midi message.
        /// </summary>
        /// <param name="recordType">A value appropriate for long midi messages.</param>
        /// <param name="buffer">The long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public MidiQueueRecord(MidiQueueRecordType recordType, MidiBufferStream buffer, int timeIndex)
        {
            //Throw.IfArgumentOutOfRange<int>((int)recordType,
            //    (int)MidiQueueRecordType.LongData, (int)MidiQueueRecordType.LongError, "recordType");

            _type = recordType;
            _data = 0;
            _buffer = buffer;
            _timeIndex = timeIndex;
        }

        private MidiQueueRecordType _type;

        /// <summary>
        /// Gets the type of record.
        /// </summary>
        public MidiQueueRecordType RecordType
        {
            get { return _type; }
        }

        private int _data;

        /// <summary>
        /// Gets the short midi message.
        /// </summary>
        public int Data
        {
            get { return _data; }
        }

        private MidiBufferStream _buffer;

        /// <summary>
        /// Gets the long midi message.
        /// </summary>
        /// <remarks>Can return null if this record was constructed for a short midi message.</remarks>
        public MidiBufferStream Buffer
        {
            get { return _buffer; }
        }

        private int _timeIndex;

        /// <summary>
        /// Gets A time indication of the midi message.
        /// </summary>
        public int TimeIndex
        {
            get { return _timeIndex; }
        }
    }
}