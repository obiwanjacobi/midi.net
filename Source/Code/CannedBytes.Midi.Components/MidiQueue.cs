namespace CannedBytes.Midi.Components
{
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// The MidiQueue class queues midi messages, both long and short.
    /// </summary>
    public class MidiQueue : DisposableBase
    {
        /// <summary>
        /// The internal queue containing the port events.
        /// </summary>
        private readonly ConcurrentQueue<MidiPortEvent> _queue = new ConcurrentQueue<MidiPortEvent>();

        /// <summary>
        /// An event to signal the extra thread to release its loop.
        /// </summary>
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);

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
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void PushShortData(int data, long timestamp)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.ShortData, data, timestamp);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as error.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void PushShortError(int data, long timestamp)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.ShortError, data, timestamp);

            Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as more-data.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void PushMoreData(int data, long timestamp)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.MoreData, data, timestamp);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue.
        /// </summary>
        /// <param name="stream">A long midi message. Must not be null.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void PushLongData(IMidiStream stream, long timestamp)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.LongData, stream, timestamp);

            Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue, marked as error.
        /// </summary>
        /// <param name="stream">A long midi message. Must not be null.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public void PushLongError(IMidiStream stream, long timestamp)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.LongError, stream, timestamp);

            Push(rec);
        }

        /// <summary>
        /// Pushes a new record onto the queue.
        /// </summary>
        /// <param name="record">A midi record.</param>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        public void Push(MidiPortEvent record)
        {
            Check.IfArgumentNull(record, nameof(record));

            _queue.Enqueue(record);

            _signal.Set();
        }

        /// <summary>
        /// De-queue's the next port events.
        /// </summary>
        /// <returns>Returns null when no event was in the queue.</returns>
        public MidiPortEvent Pop()
        {
            if (_queue.TryDequeue(out MidiPortEvent record))
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
            while (_queue.TryDequeue(out MidiPortEvent item))
            {
                // eat the items.
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
        /// <param name="millisecondsTimeout">A timeout period in milliseconds.</param>
        /// <returns>Returns true is the signal was set within the specified
        /// <paramref name="millisecondsTimeout"/> period.</returns>
        public bool Wait(int millisecondsTimeout)
        {
            return _signal.WaitOne(millisecondsTimeout, false);
        }

        /// <summary>
        /// Disposes of the internal disposables.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed &&
                disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                Clear();
                _signal.Close();
            }
        }
    }
}