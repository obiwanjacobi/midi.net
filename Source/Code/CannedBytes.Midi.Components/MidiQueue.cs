using System;
using System.Collections.Generic;
using System.Threading;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiQueue class queues midi messages, both long and short.
    /// </summary>
    public class MidiQueue : DisposableBase
    {
        private object _lock = new object();
        private AutoResetEvent _signal = new AutoResetEvent(false);
        private Queue<MidiQueueRecord> _queue = new Queue<MidiQueueRecord>(64);

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
            MidiQueueRecord rec = new MidiQueueRecord(
                MidiQueueRecordType.ShortData, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as error.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushShortError(int data, int timeIndex)
        {
            MidiQueueRecord rec = new MidiQueueRecord(
                MidiQueueRecordType.ShortError, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as more-data.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushMoreData(int data, int timeIndex)
        {
            MidiQueueRecord rec = new MidiQueueRecord(
                MidiQueueRecordType.MoreData, data, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongData(MidiBufferStream buffer, int timeIndex)
        {
            MidiQueueRecord rec = new MidiQueueRecord(
                MidiQueueRecordType.LongData, buffer, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue, marked as error.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongError(MidiBufferStream buffer, int timeIndex)
        {
            MidiQueueRecord rec = new MidiQueueRecord(
                MidiQueueRecordType.LongError, buffer, timeIndex);

            Push(rec);
        }

        /// <summary>
        /// Pushes a new record onto the queue.
        /// </summary>
        /// <param name="record">A midi record</param>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public void Push(MidiQueueRecord record)
        {
            try
            {
                Monitor.Enter(_lock);

                Console.WriteLine("Queue enqueue {0}", record.RecordType);
                _queue.Enqueue(record);

                _signal.Set();
            }
            finally
            {
                Monitor.Exit(_lock);
            }
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
        /// Pops a record of the queue.
        /// </summary>
        /// <param name="millisecsTimeout">A timeout period in milliseconds.</param>
        /// <returns>Returns an empty record if the <paramref name="millisecsTimeout"/>
        /// period elapsed.</returns>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public MidiQueueRecord Pop(int millisecsTimeout)
        {
            Nullable<MidiQueueRecord> record = null;

            try
            {
                if (Monitor.TryEnter(_lock, millisecsTimeout) &&
                    _queue.Count > 0)
                {
                    record = _queue.Dequeue();
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return record.GetValueOrDefault();
        }

        /// <summary>
        /// Clears all records from the queue.
        /// </summary>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public void Clear()
        {
            try
            {
                Monitor.Enter(_lock);
                _queue.Clear();
            }
            finally
            {
                Monitor.Exit(_lock);
            }
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
                    if (_signal != null)
                    {
                        _signal.Close();
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