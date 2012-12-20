using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace CannedBytes.Midi.RecordToFile.Midi
{
    class MidiReceiver : DisposableBase, IMidiDataReceiver
    {
        private AppData appData;
        private MidiInPort inPort;

        private MidiMessageFactory factory = new MidiMessageFactory();

        public MidiReceiver(AppData appData)
        {
            this.appData = appData;
            this.inPort = new MidiInPort();
            this.inPort.Successor = this;
        }

        public void Start(int portId)
        {
            this.appData.Events.Clear();

            this.inPort.Open(portId);
            this.inPort.Start();
        }

        public void Stop()
        {
            this.inPort.Stop();
            this.inPort.Close();
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
        }

        public void ShortData(int data, long timestamp)
        {
            var evnt = new MidiFileEvent();
            evnt.Message = this.factory.CreateShortMessage(data);
            evnt.AbsoluteTime = timestamp;

            this.appData.Events.Add(evnt);
        }

        #endregion IMidiDataReceiver Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.inPort.Dispose();
        }
    }
}