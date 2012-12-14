namespace CannedBytes.Midi.Components
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Threading;

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
        /// <summary>
        /// The event queue.
        /// </summary>
        private MidiQueue queue = new MidiQueue();

        /// <summary>
        /// The status of the midi port.
        /// </summary>
        private MidiPortStatus status = MidiPortStatus.None;

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
            Check.IfArgumentNull(buffer, "buffer");

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
            Check.IfArgumentNull(buffer, "buffer");

            this.queue.PushLongError(buffer, timeIndex);
        }

        /// <summary>
        /// Puts a Port Event on the queue.
        /// </summary>
        /// <param name="portEvent">The Port Event. Must not be null.</param>
        public void PortEvent(MidiPortEvent portEvent)
        {
            Check.IfArgumentNull(portEvent, "portEvent");

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
                   this.status != MidiPortStatus.Closed)
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

            if (this.NextReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventType.MoreData:
                    case MidiPortEventType.ShortData:
                        this.NextReceiver.ShortData(record.Data, (int)record.DeltaTime);
                        break;
                    case MidiPortEventType.LongData:
                        this.NextReceiver.LongData(record.Buffer, (int)record.DeltaTime);
                        break;
                }
            }

            if (this.NextErrorReceiver != null)
            {
                switch (record.RecordType)
                {
                    case MidiPortEventType.ShortError:
                        this.NextErrorReceiver.ShortError(record.Data, (int)record.DeltaTime);
                        break;
                    case MidiPortEventType.LongError:
                        this.NextErrorReceiver.LongError(record.Buffer, (int)record.DeltaTime);
                        break;
                }
            }

            if (this.NextPortEventReceiver != null)
            {
                this.NextPortEventReceiver.PortEvent(record);
            }
        }

        /// <summary>
        /// Event handler when the port status changes.
        /// </summary>
        /// <param name="sender">The midi port.</param>
        /// <param name="e">Not used.</param>
        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            this.NewPortStatus(port.Status);
        }

        /// <summary>
        /// Handles the new port status.
        /// </summary>
        /// <param name="newStatus">The new port status value.</param>
        private void NewPortStatus(MidiPortStatus newStatus)
        {
            if (this.status != newStatus)
            {
                this.status = newStatus;

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
        /// Initializes the receiver component with the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public void Initialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, "port");
            Check.IfArgumentNotOfType<MidiInPort>(port, "port");

            port.StatusChanged += new EventHandler(this.MidiPort_StatusChanged);
        }

        /// <summary>
        /// Removes any references the receiver component has to/from the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public void Uninitialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, "port");
            Check.IfArgumentNotOfType<MidiInPort>(port, "port");

            port.StatusChanged -= new EventHandler(this.MidiPort_StatusChanged);
            this.status = MidiPortStatus.None;
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

        #region IChainOf<IMidiDataReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextReceiver"/> property.
        /// </summary>
        private IMidiDataReceiver receiver;

        /// <summary>
        /// Gets or sets the next receiver component.
        /// </summary>
        IMidiDataReceiver IChainOf<IMidiDataReceiver>.Successor
        {
            get
            {
                return this.receiver;
            }

            set
            {
                if (this.status != MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Receiver.");
                }

                this.receiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next receiver component in the chain.
        /// </summary>
        public IMidiDataReceiver NextReceiver
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
                        "The Midi Port must be closed before setting a new value for the Successor Receiver.");
                }

                this.receiver = value;
            }
        }

        #endregion IChainOf<IMidiDataReceiver> members

        #region IChainOf<IMidiDataErrorReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextErrorReceiver"/> property.
        /// </summary>
        private IMidiDataErrorReceiver errorReceiver;

        /// <summary>
        /// The next error receiver component.
        /// </summary>
        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Successor
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
                        "The Midi Port must be closed before setting a new value for the Successor Error Receiver.");
                }

                this.errorReceiver = value;
            }
        }

        /// <summary>
        /// Gets the next error receiver component in the chain.
        /// </summary>
        public IMidiDataErrorReceiver NextErrorReceiver
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
                        "The Midi Port must be closed before setting a new value for the Successor Error Receiver.");
                }

                this.errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiDataErrorReceiver> members

        #region IChainOf<IMidiPortEventReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextPortEventReceiver"/> property.
        /// </summary>
        private IMidiPortEventReceiver portEventReceiver;

        /// <summary>
        /// The next port event receiver component.
        /// </summary>
        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Successor
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
                        "The Midi Port must be closed before setting a new value for the Successor Port Event Receiver.");
                }

                this.portEventReceiver = value;
            }
        }

        /// <summary>
        /// Gets the next port event receiver in the chain.
        /// </summary>
        public IMidiPortEventReceiver NextPortEventReceiver
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
                        "The Midi Port must be closed before setting a new value for the Successor Port Event Receiver.");
                }

                this.portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> members
    }
}