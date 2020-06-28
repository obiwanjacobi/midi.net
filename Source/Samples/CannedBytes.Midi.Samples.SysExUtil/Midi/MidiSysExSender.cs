using System.Collections.Generic;
using System.Threading;

namespace CannedBytes.Midi.Samples.SysExUtil.Midi
{
    internal sealed class MidiSysExSender : DisposableBase
    {
        private readonly MidiOutPort _outPort;

        public MidiSysExSender()
        {
            _outPort = new MidiOutPort();

            _outPort.BufferManager.Initialize(10, 1024);
        }

        public void Open(int portId)
        {
            if (!_outPort.IsOpen)
            {
                _outPort.Open(portId);
            }
        }

        public void Close()
        {
            if (_outPort.IsOpen)
            {
                _outPort.Close();
            }
        }

        public void SendAll(IEnumerable<MidiSysExBuffer> collection)
        {
            foreach (var buffer in collection)
            {
                Send(buffer);

                // little pause between buffers
                Thread.Sleep(50);
            }
        }

        public void Send(MidiSysExBuffer sysExBuffer)
        {
            var buffer = RetrieveBuffer();

            sysExBuffer.Stream.Position = 0;
            buffer.Position = 0;

            sysExBuffer.Stream.CopyTo(buffer, 0);

            _outPort.LongData(buffer);
        }

        private MidiBufferStream RetrieveBuffer()
        {
            var buffer = _outPort.BufferManager.RetrieveBuffer();

            // brute force buffer retrieval.
            while (buffer == null)
            {
                Thread.Sleep(100);

                buffer = _outPort.BufferManager.RetrieveBuffer();
            }

            return buffer;
        }

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            Close();

            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                _outPort.Dispose();
            }
        }
    }
}
