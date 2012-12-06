using System;
using System.Diagnostics;
using System.Text;

namespace CannedBytes.Midi.Components
{
    /// <summary>
    /// This class logs performance data in a receiver chain.
    /// </summary>
    public class MidiDiagnosticReceiver : MidiDataReceiverChain, IMidiDataReceiver
    {
        /// <inheritdocs/>
        public void ShortData(int data, int timeIndex)
        {
            using (new ScopedStopWatch(ShortPerformanceData))
            {
                base.NextReceiverShortData(data, timeIndex);
            }
        }

        /// <inheritdocs/>
        public void LongData(MidiBufferStream buffer, int timeIndex)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            using (new ScopedStopWatch(LongPerformanceData))
            {
                base.NextReceiverLongData(buffer, timeIndex);
            }
        }

        private PerformanceData _shortPerformanceData = new PerformanceData();

        /// <summary>
        /// Gets or sets the performance data for short Midi messages.
        /// </summary>
        public PerformanceData ShortPerformanceData
        {
            get { return _shortPerformanceData; }
            set { _shortPerformanceData = value; }
        }

        private PerformanceData _longPerformanceData = new PerformanceData();

        /// <summary>
        /// Gets or sets the performance data for long Midi messages.
        /// </summary>
        public PerformanceData LongPerformanceData
        {
            get { return _longPerformanceData; }
            set { _longPerformanceData = value; }
        }

        /// <summary>
        /// Resets the <see cref="ShortPerformanceData"/> and the <see cref="LongPerformanceData"/> members.
        /// </summary>
        public void Reset()
        {
            if (ShortPerformanceData != null)
            {
                ShortPerformanceData.Reset();
            }

            if (LongPerformanceData != null)
            {
                LongPerformanceData.Reset();
            }
        }

        /// <summary>
        /// Returns a string containing info on the the <see cref="ShortPerformanceData"/>
        /// and the <see cref="LongPerformanceData"/> logged data.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Maintains call timings.
        /// </summary>
        public class PerformanceData
        {
            // time in milliseconds
            public long FastestCall;
            public long SlowestCall;
            public long AverageCall;
            public long TotalsCall;
            public long NumberOfCalls;
            public static long Frequency = Stopwatch.Frequency;

            /// <summary>
            /// Resets all members for a new logging run.
            /// </summary>
            public void Reset()
            {
                FastestCall = Int64.MaxValue;
                SlowestCall = 0;
                AverageCall = 0;
                TotalsCall = 0;
                NumberOfCalls = 0;
            }

            /// <summary>
            /// Adds the specified <paramref name="ticks"/> for a call.
            /// </summary>
            /// <param name="ticks"></param>
            public void AddCall(long ticks)
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

        /// <summary>
        /// Helper class to measure time.
        /// </summary>
        private class ScopedStopWatch : IDisposable
        {
            private PerformanceData _prefData;
            private Stopwatch _stopWatch;

            /// <summary>
            /// Starts the time.
            /// </summary>
            /// <param name="perfData"></param>
            public ScopedStopWatch(PerformanceData perfData)
            {
                _prefData = perfData;

                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }

            /// <summary>
            /// Stops the time.
            /// </summary>
            public void Dispose()
            {
                _stopWatch.Stop();
                _prefData.AddCall(_stopWatch.ElapsedTicks);
            }
        }
    }
}