using System;
using System.Collections.Generic;
using System.Linq;

namespace MouseChef.Analysis
{
    public class Seconds : IStats
    {
        public string Description => "Time (seconds)";
        public TimeSpan Start => TimeSpan.Zero;
        public TimeSpan End => TimeSpan.MaxValue;
        public double MinValue => 0.0;
        public double MaxValue => TimeSpan.MaxValue.TotalSeconds;
        public double MedianValue => 0.0;
        public double MeanValue => 0.0;
        public double StandardDeviation => 0.0;
        public double ValueAt(TimeSpan time) => time.TotalSeconds;

        public IEnumerable<TimePoint> DataPoints => Enumerable.Empty<TimePoint>();
        public double ExpectedMinimum => 0.0;
        public double? MinorStep => 1.0;
        public double MajorStep => 10.0;
        public double ExpectedMaximum => 30.0;
    }
}
