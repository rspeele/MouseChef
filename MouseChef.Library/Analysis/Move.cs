using System;

namespace MouseChef.Analysis
{
    public struct Move
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000L;

        public Move(Mouse mouse, TimeSpan time, double dx, double dy)
        {
            Mouse = mouse;
            Time = time;
            Dx = dx;
            Dy = dy;
        }
        public Move(Mouse mouse, long us, double dx, double dy)
            :this(mouse, TimeSpan.FromTicks(us * TicksPerMicrosecond), dx, dy)
        {
        }

        public Mouse Mouse { get; }
        public TimeSpan Time { get; }
        public double Dx { get; }
        public double Dy { get; }

        public Move WithTime(TimeSpan newTime) => new Move(Mouse, newTime, Dx, Dy);
        public Move WithDxDy(double newDx, double newDy) => new Move(Mouse, Time, newDx, newDy);
    }
}
