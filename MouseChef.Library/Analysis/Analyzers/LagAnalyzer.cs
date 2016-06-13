using System;
using System.Collections.Generic;

namespace MouseChef.Analysis.Analyzers
{
    public class LagAnalyzer : IAnalyzer
    {
        private const string AnalyzerDescription =
            "Checks for a lag difference between the mice by looking at the delay"
            + " between reports of initial movement following dead stops.";

        private const string StatDescription =
            "The number of milliseconds of delay (may be negative) before the subject mouse"
            + " moves from a dead stop, relative to the baseline mouse.";

        // We consider a mouse stopped when we haven't heard from it in this long.
        private static readonly TimeSpan DeadStop = TimeSpan.FromSeconds(0.25);

        public string Name => "Lag Time (ms)";
        public string Description => AnalyzerDescription;
        public double DefaultFactor => 0.0;
        public bool AllowOverrideFactor => true;

        private class MouseLagProcessor
        {
            private readonly PointStats _stats;
            private readonly Mouse _mouse;
            private readonly bool _isBaseline;
            private TimeSpan? _lastMove = null;

            public MouseLagProcessor(Mouse mouse, PointStats stats, bool isBaseline)
            {
                _mouse = mouse;
                _stats = stats;
                _isBaseline = isBaseline;
            }

            private TimeSpan? _lastMoveFromDeadStop;

            private void CompareMovementFromDeadStop(TimeSpan moveTime, MouseLagProcessor otherMouse)
            {
                if (otherMouse._lastMoveFromDeadStop == null) return;
                var ourDelay = moveTime - otherMouse._lastMoveFromDeadStop.Value;
                if (ourDelay >= DeadStop) return; // this is from an older move
                var subjectDelay =
                    _isBaseline ? -ourDelay : ourDelay;
                _stats.AddPoint(moveTime, subjectDelay.TotalMilliseconds);
            }

            public void ProcessMove(Move move, MouseLagProcessor otherMouse)
            {
                if (move.Mouse != _mouse) return;
                var movingFromDeadStop = _lastMove != null && _lastMove < move.Time - DeadStop;
                if (movingFromDeadStop)
                {
                    CompareMovementFromDeadStop(move.Time, otherMouse);
                    _lastMoveFromDeadStop = move.Time;
                }
                _lastMove = move.Time;
            }
        }

        public IStats Analyze(Mouse baseline, Mouse subject, IEnumerable<Move> moves)
        {
            var stats = new PointStats
                ( StatDescription
                , DefaultFactor
                , -250.0
                , 250.0
                , 100.0
                , 10.0
                );

            var baseAnalyzer = new MouseLagProcessor(baseline, stats, isBaseline: true);
            var subjectAnalyzer = new MouseLagProcessor(subject, stats, isBaseline: false);
            foreach (var move in moves)
            {
                baseAnalyzer.ProcessMove(move, subjectAnalyzer);
                subjectAnalyzer.ProcessMove(move, baseAnalyzer);
            }

            return stats;
        }

        public Move TransformMove(Move move, double factor)
            => move.WithTime(move.Time - TimeSpan.FromMilliseconds(factor));
    }
}
