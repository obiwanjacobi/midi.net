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
            using (new ScopedStopWatch(this.ShortPerformanceData))
            {
                NextReceiverShortData(data, timestamp);
            }
        }

        /// <inheritdocs/>
        public void LongData(MidiBufferStream buffer, long timestamp)
        {
            Check.IfArgumentNull(buffer, "buffer");

            using (new ScopedStopWatch(this.LongPerformanceData))
            {
                NextReceiverLongData(buffer, timestamp);
            }
        }

        /// <summary>
        /// Backing field for the <see cref="ShortPerformanceData"/> property.
        /// </summary>
        private PerformanceData shortPerformanceData = new PerformanceData();

        /// <summary>
        /// Gets or sets the performance data for short Midi messages.
        /// </summary>
        public PerformanceData ShortPerformanceData
        {
            get { return this.shortPerformanceData; }
            set { this.shortPerformanceData = value; }
        }

        /// <summary>
        /// Backing field for the <see cref="LongPerformanceData"/> property.
        /// </summary>
        private PerformanceData longPerformanceData = new PerformanceData();

        /// <summary>
        /// Gets or sets the performance data for long Midi messages.
        /// </summary>
        public PerformanceData LongPerformanceData
        {
            get { return this.longPerformanceData; }
            set { this.longPerformanceData = value; }
        }

        /// <summary>
        /// Resets the <see cref="ShortPerformanceData"/> and the <see cref="LongPerformanceData"/> members.
        /// </summary>
        public void Reset()
        {
            if (this.ShortPerformanceData != null)
            {
                this.ShortPerformanceData.Reset();
            }

            if (this.LongPerformanceData != null)
            {
                this.LongPerformanceData.Reset();
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

            if (this.ShortPerformanceData != null && this.ShortPerformanceData.NumberOfCalls > 0)
            {
                txt.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "ShortData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
                    ((float)this.ShortPerformanceData.FastestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)this.ShortPerformanceData.SlowestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)this.ShortPerformanceData.AverageCall / (float)PerformanceData.Frequency) * 1000,
                    this.ShortPerformanceData.NumberOfCalls);
            }
            else
            {
                txt.Append("ShortData: <no data>");
            }

            if (this.LongPerformanceData != null && this.LongPerformanceData.NumberOfCalls > 0)
            {
                txt.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "LongData: Fastest:{0}ms Average:{2}ms Slowest:{1}ms ({3})\n",
                    ((float)this.LongPerformanceData.FastestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)this.LongPerformanceData.SlowestCall / (float)PerformanceData.Frequency) * 1000,
                    ((float)this.LongPerformanceData.AverageCall / (float)PerformanceData.Frequency) * 1000,
                    this.LongPerformanceData.NumberOfCalls);
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
        private class ScopedStopWatch : IDisposable
        {
            /// <summary>Performance data to record the time in.</summary>
            private PerformanceData perfData;

            /// <summary>Stopwatch for measuring time.</summary>
            private Stopwatch stopWatch;

            /// <summary>
            /// Starts the time.
            /// </summary>
            /// <param name="perfData">Must not be null.</param>
            public ScopedStopWatch(PerformanceData perfData)
            {
                this.perfData = perfData;

                this.stopWatch = new Stopwatch();
                this.stopWatch.Start();
            }

            /// <summary>
            /// Stops the time.
            /// </summary>
            public void Dispose()
            {
                this.stopWatch.Stop();
                this.perfData.AddCall(this.stopWatch.ElapsedTicks);
            }
        }
    }
}