namespace CannedBytes.Midi.Components
{
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// The MidiQueue class queues midi messages, both long and short.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "It is a queue, so lets call it a queue.")]
    public class MidiQueue : DisposableBase
    {
        /// <summary>
        /// The internal queue containing the port events.
        /// </summary>
        private ConcurrentQueue<MidiPortEvent> queue = new ConcurrentQueue<MidiPortEvent>();

        /// <summary>
        /// An event to signal the extra thread to release its loop.
        /// </summary>
        private AutoResetEvent signal = new AutoResetEvent(false);

        /// <summary>
        /// The object's invariant contract.
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(this.queue != null);
            Contract.Invariant(this.signal != null);
        }

        /// <summary>
        /// Returns the number of messages in the queue.
        /// </summary>
        public int Count
        {
            get { return this.queue.Count; }
        }

        /// <summary>
        /// Pushes a short midi message into the queue.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushShortData(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.ShortData, data, timeIndex);

            this.Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as error.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushShortError(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.ShortError, data, timeIndex);

            this.Push(rec);
        }

        /// <summary>
        /// Pushes a short midi message into the queue, marked as more-data.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushMoreData(int data, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.MoreData, data, timeIndex);

            this.Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongData(MidiBufferStream buffer, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.LongData, buffer, timeIndex);

            this.Push(rec);
        }

        /// <summary>
        /// Pushes a long midi message into the queue, marked as error.
        /// </summary>
        /// <param name="buffer">A long midi message. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void PushLongError(MidiBufferStream buffer, int timeIndex)
        {
            MidiPortEvent rec = new MidiPortEvent(
                MidiPortEventType.LongError, buffer, timeIndex);

            this.Push(rec);
        }

        /// <summary>
        /// Pushes a new record onto the queue.
        /// </summary>
        /// <param name="record">A midi record.</param>
        /// <remarks>This method synchronizes access to the internal queue.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Throw is not recognized.")]
        public void Push(MidiPortEvent record)
        {
            Throw.IfArgumentNull(record, "record");

            this.queue.Enqueue(record);

            this.signal.Set();
        }

        /// <summary>
        /// De-queue's the next port events.
        /// </summary>
        /// <returns>Returns null when no event was in the queue.</returns>
        public MidiPortEvent Pop()
        {
            MidiPortEvent record = null;

            if (this.queue.TryDequeue(out record))
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

            while (this.queue.TryDequeue(out item))
            {
                // eat the items.
            }
        }

        /// <summary>
        /// Sets the signal.
        /// </summary>
        public void SignalWait()
        {
            this.signal.Set();
        }

        /// <summary>
        /// Waits for the signal to be set.
        /// </summary>
        /// <param name="millisecondsTimeout">A timeout period in milliseconds.</param>
        /// <returns>Returns true is the signal was set within the specified
        /// <paramref name="millisecondsTimeout"/> period.</returns>
        public bool Wait(int millisecondsTimeout)
        {
            return this.signal.WaitOne(millisecondsTimeout, false);
        }

        /// <summary>
        /// Disposes of the internal disposables.
        /// </summary>
        /// <param name="disposing">True when also the managed objects needs disposing.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.Clear();
                    this.signal.Close();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}