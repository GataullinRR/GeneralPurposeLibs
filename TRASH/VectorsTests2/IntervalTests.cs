using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vectors;

namespace VectorsTests
{
    [TestClass]
    public class IntervalTests
    {
        [TestMethod]
        public void Touches_Test()
        {
            var a = new Interval(3, 10);
            var bInA = new Interval(3, 5);
            var bIntersectARight = new Interval(5, 15);
            var bIntersectALeft = new Interval(-1, 5);
            var bContainsA = new Interval(-1, 15);
            var bApart = new Interval(-1, 2);

            var ok = a.Touches(bInA) && a.Touches(bIntersectARight) && a.Touches(bContainsA) &&
                a.Touches(bIntersectALeft) && !a.Touches(bApart);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void OverlapLen_Test()
        {
            var a = new IntInterval(3, 10);
            var bInA = new IntInterval(3, 5);
            var bIntersectARight = new IntInterval(5, 15);
            var bIntersectALeft = new IntInterval(-1, 5);
            var bBoundARight = new IntInterval(10, 15);
            var bBoundALeft = new IntInterval(-5, 3);
            var bContainsA = new IntInterval(-1, 15);
            var bApart = new IntInterval(-1, 2);

            Assert.AreEqual(2, a.OverlapLen(bInA));
            Assert.AreEqual(5, a.OverlapLen(bIntersectARight));
            Assert.AreEqual(2, a.OverlapLen(bIntersectALeft));
            Assert.AreEqual(0, a.OverlapLen(bBoundARight));
            Assert.AreEqual(0, a.OverlapLen(bBoundALeft));
            Assert.AreEqual(7, a.OverlapLen(bContainsA));
            Assert.AreEqual(0, a.OverlapLen(bApart));
        }
    }
}
