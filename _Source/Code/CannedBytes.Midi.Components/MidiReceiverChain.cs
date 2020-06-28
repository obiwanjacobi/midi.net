namespace CannedBytes.Midi.Components
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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

            this.OnNewPortStatus(port.Status);
        }

        /// <summary>
        /// Handles the new port status.
        /// </summary>
        /// <param name="newStatus">The new port status value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "NewPort")]
        protected virtual void OnNewPortStatus(MidiPortStatus newStatus)
        {
            this.PortStatus = newStatus;
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
            this.PortStatus = MidiPortStatus.None;
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
            get { return this.NextReceiver; }
            set { this.NextReceiver = value; }
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
                if (this.PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Receiver.");
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
            get { return this.NextErrorReceiver; }
            set { this.NextErrorReceiver = value; }
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
                if (this.PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Error Receiver.");
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
            get { return this.NextPortEventReceiver; }
            set { this.NextPortEventReceiver = value; }
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
                if (this.PortStatus == MidiPortStatus.Started)
                {
                    throw new InvalidOperationException(
                        "The Midi Port must be stopped before setting a new value for the Successor Port Event Receiver.");
                }

                this.portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> members

        /// <inheritdocs/>
        public abstract void ShortData(int data, long timestamp);

        /// <inheritdocs/>
        public abstract void LongData(MidiBufferStream buffer, long timestamp);

        /// <inheritdocs/>
        public abstract void ShortError(int data, long timestamp);

        /// <inheritdocs/>
        public abstract void LongError(MidiBufferStream buffer, long timestamp);

        /// <inheritdocs/>
        public abstract void PortEvent(MidiPortEvent midiEvent);
    }
}