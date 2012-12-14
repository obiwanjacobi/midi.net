namespace CannedBytes.Midi.Components
{
    using System.Diagnostics.CodeAnalysis;
    using CannedBytes.Media;

    /// <summary>
    /// A midi receiver class that timestamps messages with ticks from a <see cref="TickTimer"/>.
    /// </summary>
    public class TimerBasedTimestampReceiver : MidiReceiverChain
    {
        /// <summary>The tick timer.</summary>
        private TickTimer timer = new TickTimer();

        /// <summary>
        /// Retrieves a new tick value.
        /// </summary>
        /// <returns>Returns the new value.</returns>
        private long GetCurrentTimestamp()
        {
            return this.timer.Ticks;
        }

        /// <summary>
        /// Manages turning the timer on and off based on the port status.
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
                        timer.StartTimer();
                        break;
                    case MidiPortStatus.Closed:
                        timer.StopTimer();
                        break;
                }
            }
        }

        /// <summary>
        /// Receives a short midi data message.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timestamp">Not used. A new timestamp is generated.</param>
        public override void ShortData(int data, long timestamp)
        {
            if (NextReceiver != null)
            {
                NextReceiver.ShortData(data, GetCurrentTimestamp());
            }
        }

        /// <summary>
        /// Receives a long midi data message.
        /// </summary>
        /// <param name="buffer">The buffer containing the long message data.</param>
        /// <param name="timestamp">Not used. A new timestamp is generated.</param>
        public override void LongData(MidiBufferStream buffer, long timestamp)
        {
            if (NextReceiver != null)
            {
                NextReceiver.LongData(buffer, GetCurrentTimestamp());
            }
        }

        /// <summary>
        /// Receives a short midi error message.
        /// </summary>
        /// <param name="data">The short error message data.</param>
        /// <param name="timestamp">Not used. A new timestamp is generated.</param>
        public override void ShortError(int data, long timestamp)
        {
            if (NextErrorReceiver != null)
            {
                NextErrorReceiver.ShortError(data, GetCurrentTimestamp());
            }
        }

        /// <summary>
        /// Receives a long midi error message.
        /// </summary>
        /// <param name="buffer">The buffer containing the long midi message error.</param>
        /// <param name="timestamp">Not used. A new timestamp is generated.</param>
        public override void LongError(MidiBufferStream buffer, long timestamp)
        {
            if (NextErrorReceiver != null)
            {
                NextErrorReceiver.LongError(buffer, GetCurrentTimestamp());
            }
        }

        /// <summary>
        /// Receives a port event.
        /// </summary>
        /// <param name="midiEvent">The port event. Must not be null.</param>
        /// <remarks>A new event is created with a new timestamp and send to the component's successors.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public override void PortEvent(MidiPortEvent midiEvent)
        {
            Check.IfArgumentNull(midiEvent, "midiEvent");

            if (NextPortEventReceiver != null)
            {
                MidiPortEvent newEvent = null;

                if (midiEvent.IsShortMessage)
                {
                    newEvent = new MidiPortEvent(midiEvent.RecordType, midiEvent.Data, GetCurrentTimestamp());
                }
                else
                {
                    newEvent = new MidiPortEvent(midiEvent.RecordType, midiEvent.Buffer, GetCurrentTimestamp());
                }

                NextPortEventReceiver.PortEvent(newEvent);
            }
        }

        /// <summary>
        /// Disposes the object instance and its internal timer.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed)
            {
                this.timer.Dispose();
            }
        }
    }
}