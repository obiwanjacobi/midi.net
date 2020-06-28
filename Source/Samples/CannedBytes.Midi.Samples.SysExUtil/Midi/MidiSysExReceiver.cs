using CannedBytes.Midi.Components;
using System;
using System.Diagnostics;

namespace CannedBytes.Midi.Samples.SysExUtil.Midi
{
    internal sealed class MidiSysExReceiver : DisposableBase, IMidiDataReceiver, IMidiDataErrorReceiver
    {
        private readonly AppData _appData;
        private readonly MidiInPort _inPort;
        private readonly MidiInPortChainManager _chainMgr;

        public MidiSysExReceiver(AppData appData)
        {
            _appData = appData;
            _inPort = new MidiInPort();
            _chainMgr = new MidiInPortChainManager(_inPort);

            _chainMgr.Add(this);

            // also hookup error notification
            _inPort.NextErrorReceiver = this;

            _inPort.BufferManager.Initialize(10, 1024);
        }

        public void Start(int portId)
        {
            _inPort.Open(portId);
            _inPort.Start();
        }

        public void Stop()
        {
            _inPort.Stop();
            _inPort.Close();
        }

        private void ScheduleAddBuffer(MidiSysExBuffer buffer)
        {
            _appData.Dispatcher.Invoke(new Action(() => DispatchedAddBuffer(buffer)));
        }

        private void DispatchedAddBuffer(MidiSysExBuffer buffer)
        {
            _appData.SysExBuffers.Add(buffer);
        }

        #region IMidiDataReceiver Members

        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            Trace.WriteLine("Receiving buffer: " + buffer.BytesRecorded);

            var sysExBuffer = MidiSysExBuffer.From(buffer);

            ScheduleAddBuffer(sysExBuffer);
        }

        public void ShortData(int data, long timestamp)
        {
            // no op
        }

        #endregion IMidiDataReceiver Members

        #region IMidiDataErrorReceiver Members

        public void LongError(MidiBufferStream buffer, long timestamp)
        {
            _appData.ReceiveErrorCount++;
        }

        public void ShortError(int data, long timestamp)
        {
            // we're doing sys-ex - not regular midi data.
        }

        #endregion IMidiDataErrorReceiver Members

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                _inPort.Dispose();
            }
        }
    }
}
