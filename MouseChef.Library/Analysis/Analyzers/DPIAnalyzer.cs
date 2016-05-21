using System;
using System.Collections.Generic;

namespace MouseChef.Analysis.Analyzers
{
    public class DPIAnalyzer : IAnalyzer
    {
        private const string AnalyzerDescription =
            "Estimates the difference in DPI between the two mice by checking"
            + " the difference in their reported distance traveled at discrete steps.";

        private const string StatDescription =
            "Ratio of subject DPI / baseline DPI.";

        // We record a data point each time all mice have traveled at least this much in their own units.
        private const double DistanceInterval = 50.0;

        public string Description => AnalyzerDescription;

        public IStats Analyze(Mouse baseline, Mouse subject, IEnumerable<Move> moves)
        {
            var stats = new PointStats(StatDescription);

            var baseD = 0.0;
            var subjD = 0.0;
            var crossTime = TimeSpan.MaxValue;
            foreach (var move in moves)
            {
                if (move.Time > crossTime)
                {
                    var ratio = subjD / baseD;
                    stats.AddPoint(crossTime, ratio);
                    // Reset trackers.
                    baseD = 0.0;
                    subjD = 0.0;
                    crossTime = TimeSpan.MaxValue;
                }
                if (move.Mouse == baseline)
                {
                    baseD += move.D.Magnitude;
                }
                else if (move.Mouse == subject)
                {
                    subjD += move.D.Magnitude;
                }
                if (baseD >= DistanceInterval
                    && subjD >= DistanceInterval)
                {
                    // We've crossed the threshold - mark the time.
                    // We'll keep eating moves until we find one later than this timestamp then add a datapoint.
                    crossTime = move.Time;
                }
            }

            return stats;
        }

        public Move TransformMove(Move move, double factor) => move.WithDelta(move.D.Div(factor));
    }
}
