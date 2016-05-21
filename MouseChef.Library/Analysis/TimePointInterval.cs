using System;

namespace MouseChef.Analysis
{
    public struct TimePointInterval
    {
        public TimePointInterval(TimePoint both)
            : this(both, both)
        {
        }
        public TimePointInterval(TimePoint start, TimePoint end)
        {
            Start = start;
            End = end;
        }

        public TimePoint Start { get; }
        public TimePoint End { get; }

        public TimeSpan DeltaTime => End.Time - Start.Time;
        public double DeltaValue => End.Value - Start.Value;

        public double InterpolateValue(TimeSpan time)
        {
            if (time <= Start.Time) return Start.Value;
            if (time >= End.Time) return End.Value;
            var ticksIntoRange = (double)(time - Start.Time).Ticks;
            var totalTicksOfRange = (double)DeltaTime.Ticks;
            var lerpFactor = ticksIntoRange / totalTicksOfRange;
            return Start.Value + lerpFactor * DeltaValue;
        }
    }
}