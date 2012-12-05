using System;

namespace CannedBytes.Midi
{
    /// <summary>
    /// Event arguments used in the sender callback.
    /// </summary>
    /// <remarks>After construction the instance is immutable.</remarks>
    public class MidiDataSenderEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="notificationStatus">The notification status.</param>
        /// <param name="buffer">The midi buffer concerned. Must not be null.</param>
        public MidiDataSenderEventArgs(
            MidiPortNotificationStatus notificationStatus,
            MidiBufferStream buffer)
        {
            //Contract.Requires<ArgumentNullException>(buffer != null);

            _status = notificationStatus;
            _buffer = buffer;
        }

        private MidiPortNotificationStatus _status;

        /// <summary>
        /// Gets the notification status.
        /// </summary>
        public MidiPortNotificationStatus NotificationStatus
        {
            get { return _status; }
        }

        private MidiBufferStream _buffer;

        /// <summary>
        /// Gets the midi buffer concerned.
        /// </summary>
        public MidiBufferStream Buffer
        {
            get { return _buffer; }
        }
    }
}