using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiQueue class queues midi messages, both long and short.
    /// </summary>
    public class MidiQueue : DisposableBase
    {
        private ConcurrentQueue<MidiPortEvent> _queue = new ConcurrentQueue<MidiPortEvent>();
        private AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// Returns the number of messages in the queue.
        /// </summary>
        public int Count
        {
            get { return _queue.Count; }
        }

        /// <summary>
        /// Pushes a short midi message into the queue.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushShortData(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventTypes.ShortData, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as error.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushShortError(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventTypes.ShortError, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as more-data.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushMoreData(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventTypes.MoreData, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongData(MidiBufferStream buffer, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventTypes.LongData, buffer, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue, marked as error.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongError(MidiBufferStream buffer, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventTypes.LongError, buffer, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a new record onto the queue.
        /// </summary>
        /// <param name="record">A midi record</param>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public void Push(MidiPortEvent record)
        {
            Debug.WriteLine("Queue adding {0}", record.RecordType);
            Push(record);

            _signal.Set();
        }

        public MidiPortEvent Pop()
        {
            MidiPortEvent record = null;

            if (_queue.TryDequeue(out record))
            {
                return record;
            }

            return null;
        }

        /// <summary>
        /// Clears all records from the queue.
        /// </summary>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public void Clear()
        {
            MidiPortEvent item = null;

            while (_queue.TryDequeue(out item))
            { }
        }

        /// <summary>
        /// Sets the signal.
        /// </summary>
        public void SignalWait()
        {
            _signal.Set();
        }

        /// <summary>
        /// Waits for the signal to be set.
        /// </summary>
        /// <param name="millisecsTimeout">A timeout period in milliseconds.</param>
        /// <returns>Returns true is the signal was set within the specified
        /// <paramref name="millisecsTimeout"/> period.</returns>
        public bool Wait(int millisecsTimeout)
        {
            return _signal.WaitOne(millisecsTimeout, false);
        }

        /// <summary>
        /// Disposes of the internal disposables.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Clear();

                    if (_signal != null)
                    {
                        _signal.Close();
                        _signal = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}