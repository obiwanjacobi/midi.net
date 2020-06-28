namespace CannedBytes.Midi.Components
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Maintains call timings.
    /// </summary>
    public class PerformanceData
    {
        /// <summary>
        /// Backing field for the <see cref="FastestCall"/> property.
        /// </summary>
        private long fastestCall;

        /// <summary>Time of the fastest call.</summary>
        public long FastestCall
        {
            get { return fastestCall; }
        }

        /// <summary>
        /// Backing field for the <see cref="SlowestCall"/> property.
        /// </summary>
        private long slowestCall;

        /// <summary>Time of the slowest call.</summary>
        public long SlowestCall
        {
            get { return slowestCall; }
        }

        /// <summary>
        /// Backing field for the <see cref="AverageCall"/> property.
        /// </summary>
        private long averageCall;

        /// <summary>Average time of the calls.</summary>
        public long AverageCall
        {
            get { return averageCall; }
        }

        /// <summary>Total time of all calls.</summary>
        private long totalsCall;

        /// <summary>
        /// Backing field for the <see cref="NumberOfCalls"/> property.
        /// </summary>
        private long numberOfCalls;

        /// <summary>Total number of calls.</summary>
        public long NumberOfCalls
        {
            get { return numberOfCalls; }
        }

        /// <summary>The can be used to convert the call times to seconds.</summary>
        public static readonly long Frequency = Stopwatch.Frequency;

        /// <summary>
        /// Resets all members for a new logging run.
        /// </summary>
        public void Reset()
        {
            fastestCall = Int64.MaxValue;
            slowestCall = 0;
            averageCall = 0;
            totalsCall = 0;
            numberOfCalls = 0;
        }

        /// <summary>
        /// Adds the specified <paramref name="ticks"/> for a call.
        /// </summary>
        /// <param name="ticks">The number of ticks of the call duration.</param>
        public void AddCall(long ticks)
        {
            if (fastestCall > ticks)
            {
                fastestCall = ticks;
            }

            if (slowestCall < ticks)
            {
                slowestCall = ticks;
            }

            numberOfCalls++;
            totalsCall += ticks;

            averageCall = totalsCall / numberOfCalls;
        }
    }
}