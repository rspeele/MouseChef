﻿using System;
using System.Collections.Generic;

namespace MouseChef.Analysis.Analyzers
{
    public class AngleAnalyzer : IAnalyzer
    {
        private const string AnalyzerDescription =
            "Estimates the difference in orientation between the two mice by checking"
            + " the difference in their displacement at discrete displacement steps.";

        private const string StatDescription =
            "Radians of rotation of the subject mouse relative to the baseline mouse.";

        // We record a data point each time all mice have displaced by at least this much in their own units.
        private const double DisplacementInterval = 100.0;

        public string Name => "Angle Offset (rads)";
        public string Description => AnalyzerDescription;
        public double DefaultFactor => 0.0;
        public bool AllowOverrideFactor => true;

        public IStats Analyze(Mouse baseline, Mouse subject, IEnumerable<Move> moves)
        {
            var stats = new PointStats
                ( StatDescription
                , DefaultFactor
                , -Math.PI
                , Math.PI
                , 0.1
                );

            const double threshold = DisplacementInterval * DisplacementInterval;
            var baseD = Vec.Zero;
            var subjD = Vec.Zero;
            var crossTime = TimeSpan.MaxValue;
            foreach (var move in moves)
            {
                if (move.Time > crossTime)
                {
                    if (baseD.MagnitudeSquared > threshold && subjD.MagnitudeSquared > threshold)
                    {
                        var angleDelta = subjD.Angle - baseD.Angle;
                        // normalize to range [-pi, pi]
                        angleDelta = Math.Atan2(Math.Sin(angleDelta), Math.Cos(angleDelta));
                        stats.AddPoint(crossTime, angleDelta);
                    }
                    // Reset trackers.
                    baseD = Vec.Zero;
                    subjD = Vec.Zero;
                    crossTime = TimeSpan.MaxValue;
                }
                if (move.Mouse == baseline)
                {
                    baseD += move.D;
                }
                else if (move.Mouse == subject)
                {
                    subjD += move.D;
                }
                if (baseD.MagnitudeSquared >= threshold
                    && subjD.MagnitudeSquared >= threshold)
                {
                    // We've crossed the threshold - mark the time.
                    // We'll keep eating moves until we find one later than this timestamp then add a datapoint.
                    crossTime = move.Time;
                }
            }

            return stats;
        }

        public Move TransformMove(Move move, double factor) => move.WithDelta(move.D.Rotate(-factor));
    }
}
