using System;
using System.Collections.Generic;

namespace MouseChef.Analysis
{
    public interface IStats
    {
        string Description { get; }
        TimeSpan Start { get; }
        TimeSpan End { get; }
        double MinValue { get; }
        double MaxValue { get; }
        double MedianValue { get; }
        double MeanValue { get; }
        double StandardDeviation { get; }
        double ValueAt(TimeSpan time);
        IEnumerable<TimePoint> DataPoints { get; }

        double ExpectedMinimum { get; }
        double? MinorStep { get; }
        double MajorStep { get; }
        double ExpectedMaximum { get; }
    }
}