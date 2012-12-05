using System;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiRunningStatusSender implements a "running status" algorithm used for
    /// sending short midi messages.
    /// </summary>
    /// <remarks>Typically this sender chain component is positioned right before the
    /// Midi Out Port because it can alter the output of short midi messages. Other
    /// chain components might not be able to handle the different format.</remarks>
    public class MidiRunningStatusSender : MidiSenderChain, IMidiDataSender, IInitializeByMidiPort
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        public MidiRunningStatusSender()
        {
            EnableRunningStatus = true;
        }

        private byte _runningStatus;

        /// <summary>
        /// Gets the current running status.
        /// </summary>
        public byte RunningStatus
        {
            get { return _runningStatus; }
        }

        private bool _enableRunningStatus;

        /// <summary>
        /// Gets or sets a value that enables or disables the running status.
        /// </summary>
        /// <remarks>Setting this property will reset the running status value.</remarks>
        public bool EnableRunningStatus
        {
            get { return _enableRunningStatus; }
            set
            {
                _enableRunningStatus = value;
                _runningStatus = 0;
            }
        }

        /// <summary>
        /// This will send the short midi message to the next sender chain component.
        /// </summary>
        /// <param name="data">The sort midi message.</param>
        /// <remarks>If the status of this midi message is the same as the previous,
        /// the status is removed from the <paramref name="data"/>.</remarks>
        public void ShortData(int data)
        {
            if (EnableRunningStatus)
            {
                MidiData eventData = new MidiData(data);
                if (eventData.Status == _runningStatus)
                {
                    data = eventData.RunningStatusData;
                }
                else
                {
                    _runningStatus = eventData.Status;
                }
            }

            NextSenderShortData(data);
        }

        /// <summary>
        /// Sends the long midi message to the next sender chain component.
        /// </summary>
        /// <param name="buffer">The long midi message.</param>
        /// <remarks>Long midi messages will reset the running status.</remarks>
        public void LongData(MidiBufferStream buffer)
        {
            _runningStatus = 0;
            NextSenderLongData(buffer);
        }

        /// <summary>
        /// Initializes the sender component with the Midi Out Port.
        /// </summary>
        /// <param name="port">The Midi Out Port.</param>
        public void Initialize(IMidiPort port)
        {
            //Throw.IfArgumentNull(port, "port");
            //Throw.IfArgumentNotOfType<MidiOutPort>(port, "port");

            port.StatusChanged += new EventHandler(MidiPort_StatusChanged);
        }

        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            if (port.HasStatus(MidiPortStatus.Closed | MidiPortStatus.Reset))
            {
                _runningStatus = 0;
            }
        }
    }
}