using System;
using System.Diagnostics;
using System.Text;

namespace CannedBytes.Midi.Components
{
    public class MidiDiagnosticReceiver : MidiReceiverChain, IMidiDataReceiver
    {
        public void ShortData(int data, int timeIndex)
        {
            using (new ScopedStopWatch(ShortPerformanceData))
            {
                base.NextReceiverShortData(data, timeIndex);
            }
        }

        public void LongData(MidiBufferStream buffer, int timeIndex)
        {
            using (new ScopedStopWatch(LongPerformanceData))
            {
                base.NextReceiverLongData(buffer, timeIndex);
            }
        }

        private PerformanceData _shortPerformanceData = new PerformanceData();

        public PerformanceData ShortPerformanceData
        {
            get { return _shortPerformanceData; }
            set { _shortPerformanceData = value; }
        }

        private PerformanceData _longPerformanceData = new PerformanceData();

        public PerformanceData LongPerformanceData
        {
            get { return _longPerformanceData; }
            set { _longPerformanceData = value; }
        }

        public void Reset()
        {
            ShortPerformanceData.Reset();
            LongPerformanceData.Reset();
        }

        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();

            txt.AppendFormat("ShortData: Fastest:{0} Average:{2} Slowest:{1} ({3})\n",
                ShortPerformanceData.FastestCall,
                ShortPerformanceData.SlowestCall,
                ShortPerformanceData.AverageCall,
                ShortPerformanceData.NumberOfCalls);

            txt.AppendFormat("LongData: Fastest:{0} Average:{2} Slowest:{1} ({3})\n",
                LongPerformanceData.FastestCall,
                LongPerformanceData.SlowestCall,
                LongPerformanceData.AverageCall,
                LongPerformanceData.NumberOfCalls);

            return txt.ToString();
        }

        //---------------------------------------------------------------------

        public class PerformanceData
        {
            // time in milliseconds
            public long FastestCall;
            public long SlowestCall;
            public long AverageCall;
            public long TotalsCall;
            public long NumberOfCalls;

            public void Reset()
            {
                FastestCall = Int64.MaxValue;
                SlowestCall = 0;
                AverageCall = 0;
                TotalsCall = 0;
                NumberOfCalls = 0;
            }

            internal void AddCall(long timeInMilliseconds)
            {
                if (FastestCall > timeInMilliseconds)
                    FastestCall = timeInMilliseconds;
                if (SlowestCall < timeInMilliseconds)
                    SlowestCall = timeInMilliseconds;

                NumberOfCalls++;
                TotalsCall += timeInMilliseconds;

                AverageCall = TotalsCall / NumberOfCalls;
            }
        }

        private class ScopedStopWatch : IDisposable
        {
            private PerformanceData _prefData;
            private Stopwatch _stopWatch;

            public ScopedStopWatch(PerformanceData perfData)
            {
                _prefData = perfData;

                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }

            public void Dispose()
            {
                _stopWatch.Stop();
                _prefData.AddCall(_stopWatch.ElapsedTicks);
            }
        }
    }
}