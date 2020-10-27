namespace CannedBytes.Midi.Components
{
    using System.Threading;

    /// <summary>
    /// The MidiReceiverAsync class implements an asynchronous receiver chain component.
    /// </summary>
    /// <remarks>This class puts received midi messages in a <see cref="MidiQueue"/>.
    /// A separate <see cref="Thread"/> reads the queue and calls the next receiver component in the chain.</remarks>
    public class MidiReceiverAsync : MidiReceiverChain
    {
        /// <summary>
        /// The event queue.
        /// </summary>
        private readonly MidiQueue _queue = new MidiQueue();

        /// <summary>
        /// Indicates if there are midi messages in the internal queue.
        /// </summary>
        public bool IsEmpty
        {
            get { return _queue.Count == 0; }
        }

        /// <summary>
        /// Puts the short midi message in the queue.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void ShortData(int data, long timestamp)
        {
            _queue.PushShortData(data, timestamp);
        }

        /// <summary>
        /// Puts the long midi message in the queue.
        /// </summary>
        /// <param name="stream">The long midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void LongData(IMidiStream stream, long timestamp)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            _queue.PushLongData(stream, timestamp);
        }

        /// <summary>
        /// Puts a short midi error in the queue.
        /// </summary>
        /// <param name="data">Error data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void ShortError(int data, long timestamp)
        {
            _queue.PushShortError(data, timestamp);
        }

        /// <summary>
        /// Puts a long midi error in the queue.
        /// </summary>
        /// <param name="stream">Error buffer. Must not be null.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void LongError(IMidiStream stream, long timestamp)
        {
            Check.IfArgumentNull(stream, nameof(stream));

            _queue.PushLongError(stream, timestamp);
        }

        /// <summary>
        /// Puts a Port Event on the queue.
        /// </summary>
        /// <param name="midiEvent">The Port Event. Must not be null.</param>
        public override void PortEvent(MidiPortEvent midiEvent)
        {
            Check.IfArgumentNull(midiEvent, nameof(midiEvent));

            _queue.Push(midiEvent);
        }

        /// <summary>
        /// Manages starting and stopping the extra (thread-pool) thread used to read the queue.
        /// </summary>
        /// <param name="newStatus">The new port status to be set.</param>
        protected override void OnNewPortStatus(MidiPortStatus newStatus)
        {
            var oldStatus = PortStatus;

            base.OnNewPortStatus(newStatus);

            if (oldStatus != newStatus)
            {
                switch (newStatus)
                {
                    case MidiPortStatus.Open:
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncReadLoop));
                        break;
                    case MidiPortStatus.Closed:
                        // signal to exit worker thread
                        _queue.SignalWait();
                        break;
                }
            }
        }

        /// <summary>
        /// The thread procedure.
        /// </summary>
        /// <param name="state">Not used.</param>
        private void AsyncReadLoop(object state)
        {
            // loop until port is closed
            while (_queue.Wait(Timeout.Infinite) &&
                   PortStatus != MidiPortStatus.Closed)
            {
                while (_queue.Count > 0)
                {
                    MidiPortEvent record = _queue.Pop();

                    if (record != null)
                    {
                        DispatchRecord(record);
                    }
                }
            }

            // throw away queued records
            _queue.Clear();
        }

        /// <summary>
        /// Dispatches the <paramref name="record"/> to the appropriate receiver component.
        /// </summary>
        /// <param name="record">Must not be null.</param>
        private void DispatchRecord(MidiPortEvent record)
        {
            Check.IfArgumentNull(record, nameof(record));

            if (NextReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventType.MoreData:
                    case MidiPortEventType.ShortData:
                        NextReceiver.ShortData(record.Data, (int)record.Timestamp);
                        break;
                    case MidiPortEventType.LongData:
                        NextReceiver.LongData(record.Stream, (int)record.Timestamp);
                        break;
                }
            }

            if (NextErrorReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventType.ShortError:
                        NextErrorReceiver.ShortError(record.Data, (int)record.Timestamp);
                        break;
                    case MidiPortEventType.LongError:
                        NextErrorReceiver.LongError(record.Stream, (int)record.Timestamp);
                        break;
                }
            }

            if (NextPortEventReceiver != null)
            {
                NextPortEventReceiver.PortEvent(record);
            }
        }

        /// <summary>
        /// Disposes of the internal queue.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                _queue.Dispose();
            }
        }
    }
}