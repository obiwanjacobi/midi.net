using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.Samples.RecordToFile.Midi
{
    internal sealed class MidiReceiver : DisposableBase, IMidiDataReceiver
    {
        private readonly AppData _appData;
        private readonly MidiInPort _inPort;

        private readonly MidiMessageFactory _factory = new MidiMessageFactory();

        public MidiReceiver(AppData appData)
        {
            _appData = appData;
            _inPort = new MidiInPort
            {
                Successor = this
            };
        }

        public void Start(int portId)
        {
            _appData.Events.Clear();

            _inPort.Open(portId);
            _inPort.Start();
        }

        public void Stop()
        {
            _inPort.Stop();
            _inPort.Close();
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            // we're not doing sys-ex.
        }

        public void ShortData(int data, long timestamp)
        {
            var evnt = new MidiFileEvent
            {
                Message = _factory.CreateShortMessage(data),
                AbsoluteTime = timestamp
            };

            _appData.Events.Add(evnt);
        }

        #endregion IMidiDataReceiver Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            _inPort.Dispose();
        }
    }
}