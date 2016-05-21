using System;

namespace MouseChef.Analysis
{
    public struct Move
    {
        private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000L;
        public Move(Mouse mouse, long us, double dx, double dy)
        {
            Mouse = mouse;
            Time = TimeSpan.FromTicks(us * TicksPerMicrosecond);
            Dx = dx;
            Dy = dy;
        }

        public Mouse Mouse { get; }
        public TimeSpan Time { get; }
        public double Dx { get; }
        public double Dy { get; }
    }
}
