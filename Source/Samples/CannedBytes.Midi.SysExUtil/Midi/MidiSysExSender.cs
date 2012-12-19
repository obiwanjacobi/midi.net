using System.Collections.Generic;
using System.Threading;
using CannedBytes.IO;

namespace CannedBytes.Midi.SysExUtil.Midi
{
    class MidiSysExSender : DisposableBase
    {
        private MidiOutPort outPort;

        public MidiSysExSender()
        {
            this.outPort = new MidiOutPort();

            this.outPort.BufferManager.Initialize(10, 1024);
        }

        public void Open(int portId)
        {
            this.outPort.Open(portId);
        }

        public void Close()
        {
            this.outPort.Close();
        }

        public void SendAll(IEnumerable<MidiSysExBuffer> collection)
        {
            foreach (var buffer in collection)
            {
                Send(buffer);

                // little pause between buffers
                Thread.Sleep(20);
            }
        }

        public void Send(MidiSysExBuffer sysExBuffer)
        {
            var buffer = this.RetrieveBuffer();

            sysExBuffer.Stream.Position = 0;
            buffer.Position = 0;

            StreamHelpers.CopyTo(sysExBuffer.Stream, buffer, 0);

            this.outPort.LongData(buffer);
        }

        private MidiBufferStream RetrieveBuffer()
        {
            var buffer = this.outPort.BufferManager.RetrieveBuffer();

            // brute force buffer retrieval.
            while (buffer == null)
            {
                Thread.Sleep(100);

                buffer = this.outPort.BufferManager.RetrieveBuffer();
            }

            return buffer;
        }

        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            this.outPort.Dispose();
        }
    }
}