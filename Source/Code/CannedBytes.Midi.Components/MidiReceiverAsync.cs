namespace CannedBytes.Midi.Components
{
    using System.Diagnostics.Contracts;
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
        private MidiQueue queue = new MidiQueue();

        /// <summary>
        /// The object's invariant state.
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(this.queue != null);
        }

        /// <summary>
        /// Indicates if there are midi messages in the internal queue.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.queue.Count == 0; }
        }

        /// <summary>
        /// Puts the short midi message in the queue.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void ShortData(int data, long timestamp)
        {
            this.queue.PushShortData(data, timestamp);
        }

        /// <summary>
        /// Puts the long midi message in the queue.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void LongData(MidiBufferStream buffer, long timestamp)
        {
            Check.IfArgumentNull(buffer, "buffer");

            this.queue.PushLongData(buffer, timestamp);
        }

        /// <summary>
        /// Puts a short midi error in the queue.
        /// </summary>
        /// <param name="data">Error data.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void ShortError(int data, long timestamp)
        {
            this.queue.PushShortError(data, timestamp);
        }

        /// <summary>
        /// Puts a long midi error in the queue.
        /// </summary>
        /// <param name="buffer">Error buffer. Must not be null.</param>
        /// <param name="timestamp">A time indication of the midi message.</param>
        public override void LongError(MidiBufferStream buffer, long timestamp)
        {
            Check.IfArgumentNull(buffer, "buffer");

            this.queue.PushLongError(buffer, timestamp);
        }

        /// <summary>
        /// Puts a Port Event on the queue.
        /// </summary>
        /// <param name="portEvent">The Port Event. Must not be null.</param>
        public override void PortEvent(MidiPortEvent portEvent)
        {
            Check.IfArgumentNull(portEvent, "portEvent");

            this.queue.Push(portEvent);
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
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this.AsyncReadLoop));
                        break;
                    case MidiPortStatus.Closed:
                        // signal to exit worker thread
                        this.queue.SignalWait();
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
            while (this.queue.Wait(Timeout.Infinite) &&
                   PortStatus != MidiPortStatus.Closed)
            {
                while (this.queue.Count > 0)
                {
                    MidiPortEvent record = this.queue.Pop();

                    if (record != null)
                    {
                        this.DispatchRecord(record);
                    }
                }
            }

            // throw away queued records
            this.queue.Clear();
        }

        /// <summary>
        /// Dispatches the <paramref name="record"/> to the appropriate receiver component.
        /// </summary>
        /// <param name="record">Must not be null.</param>
        private void DispatchRecord(MidiPortEvent record)
        {
            Contract.Requires(record != null);
            Check.IfArgumentNull(record, "record");

            if (NextReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventType.MoreData:
                    case MidiPortEventType.ShortData:
                        NextReceiver.ShortData(record.Data, (int)record.Timestamp);
                        break;
                    case MidiPortEventType.LongData:
                        NextReceiver.LongData(record.Buffer, (int)record.Timestamp);
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
                        NextErrorReceiver.LongError(record.Buffer, (int)record.Timestamp);
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
                this.queue.Dispose();
            }
        }
    }
}