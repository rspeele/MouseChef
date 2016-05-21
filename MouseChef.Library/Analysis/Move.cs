using System;

namespace MouseChef.Analysis
{
    public struct Move
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000L;

        public Move(Mouse mouse, TimeSpan time, Vec d)
        {
            Mouse = mouse;
            Time = time;
            D = d;
        }
        public Move(Mouse mouse, long us, double dx, double dy)
            :this(mouse, TimeSpan.FromTicks(us * TicksPerMicrosecond), new Vec(dx, dy))
        {
        }

        public Mouse Mouse { get; }
        public TimeSpan Time { get; }
        public Vec D { get; }

        public Move WithTime(TimeSpan newTime) => new Move(Mouse, newTime, D);
        public Move WithDelta(Vec newD) => new Move(Mouse, Time, newD);
        public Move WithDxDy(double newDx, double newDy) => WithDelta(new Vec(newDx, newDy));
    }
}
