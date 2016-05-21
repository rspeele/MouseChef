using System;

namespace MouseChef
{
    public struct Vec
    {
        public Vec(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public Vec Mul(Vec o) => new Vec(X * o.X, Y * o.Y);
        public Vec Mul(double scale) => new Vec(X * scale, Y * scale);
        public Vec Div(Vec o) => new Vec(X / o.X, Y / o.Y);
        public Vec Div(double scale) => Mul(1.0 / scale);
        public Vec Add(Vec o) => new Vec(X + o.X, Y + o.Y);
        public Vec Sub(Vec o) => new Vec(X - o.X, Y - o.Y);

        /// <summary>
        /// Square of the magnitude.
        /// </summary>
        public double MagnitudeSquared => X * X + Y * Y;
        public double Magnitude => Math.Sqrt(X * X + Y * Y);
        public double Dot(Vec o) => X * o.X + Y * o.Y;

        public static Vec operator *(Vec v, Vec u) => v.Mul(u);
        public static Vec operator *(Vec v, double s) => v.Mul(s);
        public static Vec operator *(double s, Vec v) => v.Mul(s);
        public static Vec operator /(Vec v, Vec u) => v.Div(u);
        public static Vec operator /(Vec v, double s) => v.Div(s);
        public static Vec operator +(Vec v, Vec u) => v.Add(u);
        public static Vec operator -(Vec v, Vec u) => v.Sub(u);
        public static Vec operator -(Vec v) => v.Mul(-1.0);

        public double Angle => Math.Atan2(Y, X);
        public Vec Rotate(double rads)
        {
            var c = Math.Cos(rads);
            var s = Math.Sin(rads);
            return new Vec
                ( X * c - Y * s
                , Y * c + X * s);
        }

        public static readonly Vec Zero = new Vec(x: 0, y: 0);
    }
}
