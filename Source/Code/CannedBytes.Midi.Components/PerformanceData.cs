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
        private long _fastestCall;

        /// <summary>Time of the fastest call.</summary>
        public long FastestCall
        {
            get { return _fastestCall; }
        }

        /// <summary>
        /// Backing field for the <see cref="SlowestCall"/> property.
        /// </summary>
        private long _slowestCall;

        /// <summary>Time of the slowest call.</summary>
        public long SlowestCall
        {
            get { return _slowestCall; }
        }

        /// <summary>
        /// Backing field for the <see cref="AverageCall"/> property.
        /// </summary>
        private long _averageCall;

        /// <summary>Average time of the calls.</summary>
        public long AverageCall
        {
            get { return _averageCall; }
        }

        /// <summary>Total time of all calls.</summary>
        private long _totalsCall;

        /// <summary>
        /// Backing field for the <see cref="NumberOfCalls"/> property.
        /// </summary>
        private long _numberOfCalls;

        /// <summary>Total number of calls.</summary>
        public long NumberOfCalls
        {
            get { return _numberOfCalls; }
        }

        /// <summary>The can be used to convert the call times to seconds.</summary>
        public static readonly long Frequency = Stopwatch.Frequency;

        /// <summary>
        /// Resets all members for a new logging run.
        /// </summary>
        public void Reset()
        {
            _fastestCall = Int64.MaxValue;
            _slowestCall = 0;
            _averageCall = 0;
            _totalsCall = 0;
            _numberOfCalls = 0;
        }

        /// <summary>
        /// Adds the specified <paramref name="ticks"/> for a call.
        /// </summary>
        /// <param name="ticks">The number of ticks of the call duration.</param>
        public void AddCall(long ticks)
        {
            if (_fastestCall > ticks)
            {
                _fastestCall = ticks;
            }

            if (_slowestCall < ticks)
            {
                _slowestCall = ticks;
            }

            _numberOfCalls++;
            _totalsCall += ticks;

            _averageCall = _totalsCall / _numberOfCalls;
        }
    }
}