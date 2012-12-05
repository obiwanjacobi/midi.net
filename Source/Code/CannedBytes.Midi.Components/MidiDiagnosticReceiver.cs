using System;
using System.Diagnostics;
using System.Text;

namespace CannedBytes.Midi.Components
{
    public class MidiDiagnosticReceiver : MidiDataReceiverChain, IMidiDataReceiver
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

            if (ShortPerformanceData != null && ShortPerformanceData.NumberOfCalls > 0)
            {
                txt.AppendFormat("ShortData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
                    ((float)ShortPerformanceData.FastestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)ShortPerformanceData.SlowestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)ShortPerformanceData.AverageCall / (float)PerformanceData.Frequency) * 1000,
                    ShortPerformanceData.NumberOfCalls);
            }
            else
            {
                txt.Append("ShortData: <no data>");
            }

            if (LongPerformanceData != null && LongPerformanceData.NumberOfCalls > 0)
            {
                txt.AppendFormat("LongData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
                    ((float)LongPerformanceData.FastestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)LongPerformanceData.SlowestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)LongPerformanceData.AverageCall / (float)PerformanceData.Frequency) * 1000,
                    LongPerformanceData.NumberOfCalls);
            }
            else
            {
                txt.Append("LongData: <no data>");
            }

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
            public static long Frequency = Stopwatch.Frequency;

            public void Reset()
            {
                FastestCall = Int64.MaxValue;
                SlowestCall = 0;
                AverageCall = 0;
                TotalsCall = 0;
                NumberOfCalls = 0;
            }

            internal void AddCall(long ticks)
            {
                if (FastestCall > ticks)
                    FastestCall = ticks;
                if (SlowestCall < ticks)
                    SlowestCall = ticks;

                NumberOfCalls++;
                TotalsCall += ticks;

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