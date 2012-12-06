using System;
using System.Diagnostics.Contracts;
using System.Threading;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverAsync class implements an asynchronous receiver chain component.
    /// </summary>
    /// <remarks>This class puts received midi messages in a <see cref="MidiQueue"/>.
    /// A separate <see cref="Thread"/> reads the queue and calls the next receiver component in the chain.</remarks>
    public class MidiReceiverAsync : DisposableBase, IInitializeByMidiPort,
        IChainOf<IMidiDataReceiver>, IMidiDataReceiver,
        IChainOf<IMidiDataErrorReceiver>, IMidiDataErrorReceiver,
        IChainOf<IMidiPortEventReceiver>, IMidiPortEventReceiver
    {
        private MidiQueue queue = new MidiQueue();
        private MidiPortStatus status = MidiPortStatus.None;

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
            get { return (this.queue.Count == 0); }
        }

        /// <summary>
        /// Puts the short midi message in the queue.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void ShortData(int data, int timeIndex)
        {
            this.queue.PushShortData(data, timeIndex);
        }

        /// <summary>
        /// Puts the long midi message in the queue.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void LongData(MidiBufferStream buffer, int timeIndex)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            this.queue.PushLongData(buffer, timeIndex);
        }

        /// <summary>
        /// Puts a short midi error in the queue.
        /// </summary>
        /// <param name="data">Error data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void ShortError(int data, int timeIndex)
        {
            this.queue.PushShortError(data, timeIndex);
        }

        /// <summary>
        /// Puts a long midi error in the queue.
        /// </summary>
        /// <param name="buffer">Error buffer. Must not be null.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void LongError(MidiBufferStream buffer, int timeIndex)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            this.queue.PushLongError(buffer, timeIndex);
        }

        /// <summary>
        /// Puts a Port Event on the queue.
        /// </summary>
        /// <param name="portEvent">The Port Event. Must not be null.</param>
        public void PortEvent(MidiPortEvent portEvent)
        {
            Throw.IfArgumentNull(portEvent, "portEvent");

            this.queue.Push(portEvent);
        }

        /// <summary>
        /// The thread procedure.
        /// </summary>
        /// <param name="state">Not used.</param>
        private void AsyncReadLoop(object state)
        {
            // loop until port is closed
            while (this.queue.Wait(Timeout.Infinite) &&
                status != MidiPortStatus.Closed)
            {
                while (this.queue.Count > 0)
                {
                    MidiPortEvent record = this.queue.Pop();

                    if (record != null)
                    {
                        DispatchRecord(record);
                    }
                }
            }

            // throw away queued records
            this.queue.Clear();
        }

        private void DispatchRecord(MidiPortEvent record)
        {
            Contract.Requires(record != null);
            Throw.IfArgumentNull(record, "record");

            if (NextReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventTypes.MoreData:
                    case MidiPortEventTypes.ShortData:
                        NextReceiver.ShortData(record.Data, (int)record.DeltaTime);
                        break;
                    case MidiPortEventTypes.LongData:
                        NextReceiver.LongData(record.Buffer, (int)record.DeltaTime);
                        break;
                }
            }

            if (NextErrorReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventTypes.ShortError:
                        NextErrorReceiver.ShortError(record.Data, (int)record.DeltaTime);
                        break;
                    case MidiPortEventTypes.LongError:
                        NextErrorReceiver.LongError(record.Buffer, (int)record.DeltaTime);
                        break;
                }
            }

            if (NextPortEventReceiver != null)
            {
                NextPortEventReceiver.PortEvent(record);
            }
        }

        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            NewPortStatus(port.Status);
        }

        private void NewPortStatus(MidiPortStatus status)
        {
            if (this.status != status)
            {
                this.status = status;

                switch (status)
                {
                    case MidiPortStatus.Open:
                        ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncReadLoop));
                        break;
                    case MidiPortStatus.Closed:
                        // signal to exit worker thread
                        this.queue.SignalWait();
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes the receiver component with the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        public void Initialize(IMidiPort port)
        {
            Throw.IfArgumentNull(port, "port");
            Throw.IfArgumentNotOfType<MidiInPort>(port, "port");

            port.StatusChanged += new EventHandler(MidiPort_StatusChanged);
        }

        /// <summary>
        /// Removes any references the receiver component has to/from the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        public void Uninitialize(IMidiPort port)
        {
            Throw.IfArgumentNull(port, "port");
            Throw.IfArgumentNotOfType<MidiInPort>(port, "port");

            port.StatusChanged -= new EventHandler(MidiPort_StatusChanged);
            this.status = MidiPortStatus.None;
        }

        /// <summary>
        /// Disposes of the internal queue.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    this.queue.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region IChainOf<IMidiDataReceiver> members

        private IMidiDataReceiver receiver;

        IMidiDataReceiver IChainOf<IMidiDataReceiver>.Next
        {
            get
            {
                return this.receiver;
            }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextReceiver");
                }

                this.receiver = value;
            }
        }

        public IMidiDataReceiver NextReceiver
        {
            get { return this.receiver; }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextReceiver");
                }

                this.receiver = value;
            }
        }

        #endregion IChainOf<IMidiDataReceiver> members

        #region IChainOf<IMidiDataErrorReceiver> members

        private IMidiDataErrorReceiver errorReceiver;

        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Next
        {
            get
            {
                return this.errorReceiver;
            }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextErrorReceiver");
                }

                this.errorReceiver = value;
            }
        }

        public IMidiDataErrorReceiver NextErrorReceiver
        {
            get { return this.errorReceiver; }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextErrorReceiver");
                }

                this.errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiDataErrorReceiver> members

        #region IChainOf<IMidiPortEventReceiver> members

        private IMidiPortEventReceiver portEventReceiver;

        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Next
        {
            get
            {
                return this.portEventReceiver;
            }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextPortEventReceiver");
                }

                this.portEventReceiver = value;
            }
        }

        public IMidiPortEventReceiver NextPortEventReceiver
        {
            get { return this.portEventReceiver; }
            set
            {
                if (this.status != MidiPortStatus.Closed)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be closed before setting a new value for the NextPortEventReceiver");
                }

                this.portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> members
    }
}