namespace MouseChef.Corrections
{
    public struct Move
    {
        public Move(Mouse mouse, long us, double dx, double dy)
        {
            Mouse = mouse;
            Us = us;
            Dx = dx;
            Dy = dy;
        }

        public Mouse Mouse { get; }
        public long Us { get; }
        public double Dx { get; }
        public double Dy { get; }
    }
}
