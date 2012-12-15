using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CannedBytes.Midi.Components;

namespace CannedBytes.Midi.SysExUtil.Midi
{
    class MidiSysExReceiver : IMidiDataReceiver
    {
        private AppData appData;
        private MidiInPort inPort;
        private MidiInPortChainManager chainMgr;

        public MidiSysExReceiver(AppData appData)
        {
            this.appData = appData;
            this.inPort = new MidiInPort();
            this.chainMgr = new MidiInPortChainManager(this.inPort);

            this.chainMgr.Add(this);

            this.inPort.BufferManager.Initialize(10, 1024);
        }

        public void Start(int portId)
        {
            this.inPort.Open(portId);
            this.inPort.Start();
        }

        public void Stop()
        {
            this.inPort.Stop();
            this.inPort.Close();
        }

        private void ScheduleAddBuffer(MidiSysExBuffer buffer)
        {
            this.appData.Dispatcher.Invoke(new Action(() => DispatchedAddBuffer(buffer) ));
        }

        private void DispatchedAddBuffer(MidiSysExBuffer buffer)
        {
            this.appData.SysExBuffers.Add(buffer);
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            var sysExBuffer = MidiSysExBuffer.From(buffer);

            ScheduleAddBuffer(sysExBuffer);
        }

        public void ShortData(int data, long timestamp)
        {
            // no op
        }

        #endregion
    }
}
