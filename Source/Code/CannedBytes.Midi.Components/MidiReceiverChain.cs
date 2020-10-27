namespace CannedBytes.Midi.Components
{
    using System;

    /// <summary>
    /// An abstract base class that implements all receiver interfaces for a chain component.
    /// </summary>
    public abstract class MidiReceiverChain : DisposableBase, IInitializeByMidiPort,
        IChainOf<IMidiDataReceiver>, IMidiDataReceiver,
        IChainOf<IMidiDataErrorReceiver>, IMidiDataErrorReceiver,
        IChainOf<IMidiPortEventReceiver>, IMidiPortEventReceiver
    {
        /// <summary>
        /// The status of the midi port.
        /// </summary>
        protected MidiPortStatus PortStatus { get; private set; }

        /// <summary>
        /// Event handler when the port status changes.
        /// </summary>
        /// <param name="sender">The midi port.</param>
        /// <param name="e">Not used.</param>
        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            OnNewPortStatus(port.Status);
        }

        /// <summary>
        /// Handles the new port status.
        /// </summary>
        /// <param name="newStatus">The new port status value.</param>
        protected virtual void OnNewPortStatus(MidiPortStatus newStatus)
        {
            PortStatus = newStatus;
        }

        /// <summary>
        /// Initializes the receiver component with the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        public void Initialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, nameof(port));
            Check.IfArgumentNotOfType<MidiInPort>(port, nameof(port));

            port.StatusChanged += new EventHandler(MidiPort_StatusChanged);
        }

        /// <summary>
        /// Removes any references the receiver component has to/from the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        public void Uninitialize(IMidiPort port)
        {
            Check.IfArgumentNull(port, "port");
            Check.IfArgumentNotOfType<MidiInPort>(port, "port");

            port.StatusChanged -= new EventHandler(MidiPort_StatusChanged);
            PortStatus = MidiPortStatus.None;
        }

        #region IChainOf<IMidiDataReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextReceiver"/> property.
        /// </summary>
        private IMidiDataReceiver _receiver;

        /// <summary>
        /// Gets or sets the next receiver component.
        /// </summary>
        IMidiDataReceiver IChainOf<IMidiDataReceiver>.Successor
        {
            get { return NextReceiver; }
            set { NextReceiver = value; }
        }

        /// <summary>
        /// Gets or sets the next receiver component in the chain.
        /// </summary>
        public IMidiDataReceiver NextReceiver
        {
            get
            {
                return _receiver;
            }

            set
            {
                if (PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Receiver.");
                }

                _receiver = value;
            }
        }

        #endregion IChainOf<IMidiDataReceiver> members

        #region IChainOf<IMidiDataErrorReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextErrorReceiver"/> property.
        /// </summary>
        private IMidiDataErrorReceiver _errorReceiver;

        /// <summary>
        /// The next error receiver component.
        /// </summary>
        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Successor
        {
            get { return NextErrorReceiver; }
            set { NextErrorReceiver = value; }
        }

        /// <summary>
        /// Gets the next error receiver component in the chain.
        /// </summary>
        public IMidiDataErrorReceiver NextErrorReceiver
        {
            get
            {
                return _errorReceiver;
            }

            set
            {
                if (PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Error Receiver.");
                }

                _errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiDataErrorReceiver> members

        #region IChainOf<IMidiPortEventReceiver> members

        /// <summary>
        /// Backing field for the <see cref="NextPortEventReceiver"/> property.
        /// </summary>
        private IMidiPortEventReceiver _portEventReceiver;

        /// <summary>
        /// The next port event receiver component.
        /// </summary>
        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Successor
        {
            get { return NextPortEventReceiver; }
            set { NextPortEventReceiver = value; }
        }

        /// <summary>
        /// Gets the next port event receiver in the chain.
        /// </summary>
        public IMidiPortEventReceiver NextPortEventReceiver
        {
            get
            {
                return _portEventReceiver;
            }

            set
            {
                if (PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Port Event Receiver.");
                }

                _portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> members

        /// <inheritdocs/>
        public abstract void ShortData(int data, long timestamp);

        /// <inheritdocs/>
        public abstract void LongData(IMidiStream stream, long timestamp);

        /// <inheritdocs/>
        public abstract void ShortError(int data, long timestamp);

        /// <inheritdocs/>
        public abstract void LongError(IMidiStream stream, long timestamp);

        /// <inheritdocs/>
        public abstract void PortEvent(MidiPortEvent midiEvent);
    }
}