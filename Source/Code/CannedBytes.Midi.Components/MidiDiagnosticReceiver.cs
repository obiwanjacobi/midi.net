namespace CannedBytes.Midi.Components
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// This class logs performance data in a receiver chain.
    /// </summary>
    public class MidiDiagnosticReceiver : MidiDataReceiverChain, IMidiDataReceiver
    {
        /// <inheritdocs/>
        public void ShortData(int data, long timestamp)
        {
            using (new ScopedStopWatch(ShortPerformanceData))
            {
                NextReceiverShortData(data, timestamp);
            }
        }

        /// <inheritdocs/>
        public void LongData(IMidiStream stream, long timestamp)
        {
            Check.IfArgumentNull(stream, "buffer");

            using (new ScopedStopWatch(LongPerformanceData))
            {
                NextReceiverLongData(stream, timestamp);
            }
        }

        /// <summary>
        /// Gets or sets the performance data for short Midi messages.
        /// </summary>
        public PerformanceData ShortPerformanceData { get; set; }

        /// <summary>
        /// Gets or sets the performance data for long Midi messages.
        /// </summary>
        public PerformanceData LongPerformanceData { get; set; }

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
        /// Called to dispose the object instance.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            // no-op
        }

        /// <summary>
        /// Returns a string containing info on the the <see cref="ShortPerformanceData"/>
        /// and the <see cref="LongPerformanceData"/> logged data.
        /// </summary>
        /// <returns>Returns a textual representation of the performance data.</returns>
        public override string ToString()
        {
            StringBuilder txt = new StringBuilder();

            if (ShortPerformanceData != null && ShortPerformanceData.NumberOfCalls > 0)
            {
                txt.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "ShortData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
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
                txt.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "LongData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
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

        /// <summary>
        /// Helper class to measure time.
        /// </summary>
        private sealed class ScopedStopWatch : IDisposable
        {
            /// <summary>Performance data to record the time in.</summary>
            private readonly PerformanceData _perfData;

            /// <summary>Stopwatch for measuring time.</summary>
            private readonly Stopwatch _stopWatch;

            /// <summary>
            /// Starts the time.
            /// </summary>
            /// <param name="perfData">Must not be null.</param>
            public ScopedStopWatch(PerformanceData perfData)
            {
                _perfData = perfData;

                _stopWatch = new Stopwatch();
                _stopWatch.Start();
            }

            /// <summary>
            /// Stops the time.
            /// </summary>
            public void Dispose()
            {
                _stopWatch.Stop();
                _perfData.AddCall(_stopWatch.ElapsedTicks);
            }
        }
    }
}