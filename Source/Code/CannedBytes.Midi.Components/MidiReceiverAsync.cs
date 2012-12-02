using System;
using System.Threading;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// The MidiReceiverAsync class implements an asynchronous receiver chain component.
    /// </summary>
    /// <remarks>This class puts received midi messages in a <see cref="MidiQueue"/>.
    /// A separate <see cref="Thread"/> reads the queue and calls the next receiver component in the chain.</remarks>
    public class MidiReceiverAsync : MidiReceiverChain,
        IMidiReceiver, IInitializeByMidiPort
    {
        private MidiQueue _queue;
        private MidiPortStatus _status;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MidiReceiverAsync()
        {
            _queue = new MidiQueue();
        }

        /// <summary>
        /// Indicates if there are midi messages in the internal queue.
        /// </summary>
        public bool IsEmpty
        {
            get { return (_queue.Count == 0); }
        }

        /// <summary>
        /// Puts the short midi message in the queue.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void ShortData(int data, int timeIndex)
        {
            _queue.PushShortData(data, timeIndex);
        }

        /// <summary>
        /// Puts the long midi message in the queue.
        /// </summary>
        /// <param name="buffer">The long midi message data.</param>
        /// <param name="timeIndex">A time indication of the midi message.</param>
        public void LongData(MidiBufferStream buffer, int timeIndex)
        {
            _queue.PushLongData(buffer, timeIndex);
        }

        private void AsyncReadLoop(object state)
        {
            // loop until port is closed
            while (_queue.Wait(Timeout.Infinite) &&
                _status != MidiPortStatus.Closed)
            {
                while (_queue.Count > 0)
                {
                    MidiQueueRecord record = _queue.Pop(Timeout.Infinite);

                    DispatchRecord(ref record);
                }
            }

            // throw away queued records
            _queue.Clear();
        }

        private void DispatchRecord(ref MidiQueueRecord record)
        {
            switch (record.RecordType)
            {
                case MidiQueueRecordType.ShortData:
                    base.NextReceiverShortData(record.Data, record.TimeIndex);
                    break;
                case MidiQueueRecordType.LongData:
                    base.NextReceiverLongData(record.Buffer, record.TimeIndex);
                    break;
            }
        }

        private void MidiPort_StatusChanged(object sender, EventArgs e)
        {
            IMidiPort port = (IMidiPort)sender;

            NewPortStatus(port.Status);
        }

        private void NewPortStatus(MidiPortStatus status)
        {
            if (_status != status)
            {
                _status = status;

                switch (_status)
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
        /// Initializes the receiver component with the Midi Port.
        /// </summary>
        /// <param name="port">A Midi In Port. Must not be null.</param>
        public void Initialize(IMidiPort port)
        {
            //Throw.IfArgumentNull(port, "port");
            //Throw.IfArgumentNotOfType<MidiInPort>(port, "port");   // not really needed...

            port.StatusChanged += new EventHandler(MidiPort_StatusChanged);
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
                    if (_queue != null)
                    {
                        _queue.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}