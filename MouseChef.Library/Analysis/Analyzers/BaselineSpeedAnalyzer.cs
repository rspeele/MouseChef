using System;
using System.Collections.Generic;

namespace MouseChef.Analysis.Analyzers
{
    public class BaselineSpeedAnalyzer : IAnalyzer
    {
        private const string AnalyzerDescription =
            "The movement speed of the baseline mouse in counts/second.";

        private const string StatDescription =
            "Speed of the baseline mouse in counts/second.";

        // We record a data point each time the baseline mouse moves this distance.
        private const double DistanceInterval = 100.0;

        public string Name => "Baseline Speed";
        public string Description => AnalyzerDescription;
        public double DefaultFactor => 0.0;
        public bool AllowOverrideFactor => false;

        public IStats Analyze(Mouse baseline, Mouse subject, IEnumerable<Move> moves)
        {
            var stats = new PointStats
                ( StatDescription
                , DefaultFactor
                , 0.0
                , 100000.0
                , 10000.0
                , 1000.0
                );

            TimeSpan? start = null;
            var distance = 0.0;
            foreach (var move in moves)
            {
                if (move.Mouse != baseline) continue;
                if (start == null) start = move.Time;
                distance += move.D.Magnitude;
                if (distance >= DistanceInterval)
                {
                    var time = move.Time - start.Value;
                    var speed = distance / time.TotalSeconds;
                    stats.AddPoint(move.Time, speed);
                    distance = 0.0;
                    start = move.Time;
                }
            }
            return stats;
        }

        public Move TransformMove(Move move, double factor) => move;
    }
}
