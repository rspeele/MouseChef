using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MouseChef.Test
{
    [TestClass]
    public class TestVec
    {
        private static readonly Vec[] TestVecs =
        {
            new Vec(0.0, 0.0),
            new Vec(1.0, 0.0),
            new Vec(0.0, 1.0),
            new Vec(1.0, 1.0),
            new Vec(-1.0, -1.0),
            new Vec(-1.0, 0.0),
            new Vec(0.0, -1.0),
        };

        [TestMethod]
        public void TestAdd()
        {
            var v = new Vec(1.0, 1.0);
            var u = new Vec(-2.0, 3.0);
            var a = v + u;
            Assert.AreEqual(new Vec(-1.0, 4.0), a);
        }

        [TestMethod]
        public void TestMul()
        {
            var v = new Vec(3.0, 2.0);
            var a = v * 2.0;
            Assert.AreEqual(new Vec(6.0, 4.0), a);
        }

        [TestMethod]
        public void TestAngle()
        {
            var prev = new Vec(0.25, 2.0);
            foreach (var vec in TestVecs.Where(v => !v.IsZero))
            {
                var angleDiff = vec.Angle - prev.Angle;
                var rotated = vec.Rotate(-angleDiff);
                Console.WriteLine($"{prev} {rotated}");
                Assert.AreEqual(Math.Round(prev.Normal.X, 3), Math.Round(rotated.Normal.X, 3));
                prev = vec;
            }
        }
    }
}