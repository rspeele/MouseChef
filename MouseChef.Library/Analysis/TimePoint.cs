using System;

namespace MouseChef.Analysis
{
    public struct TimePoint
    {
        public TimePoint(TimeSpan time, double value)
        {
            Time = time;
            Value = value;
        }

        public TimeSpan Time { get; }
        public double Value { get; }
    }
}