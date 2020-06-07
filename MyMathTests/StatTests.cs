using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Vectors;

namespace MyMath.Tests
{
    [TestClass()]
    public class StatTests
    {
        [TestMethod()]
        public void CalcPirsonCorrelationTest()
        {
            List<V2> seq = new List<V2>() {
                new V2(19, 17),
                new V2(32, 7),
                new V2(33, 17),
                new V2(44, 28),
                new V2(28, 27),
                new V2(35, 31),
                new V2(39, 20),
                new V2(39, 17),
                new V2(44, 35),
                new V2(44, 43),
                new V2(24, 10),
                new V2(37, 28),
                new V2(29, 13),
                new V2(40, 43),
                new V2(42, 45),
                new V2(32, 24),
                new V2(48, 43),
                new V2(42, 26),
                new V2(33, 16),
                new V2(47, 26)
            };

            double actual = Statistic.CalcPirsonCorrelation(seq).Round(2);
            double expected = 0.67;

            Assert.AreEqual(expected, actual);
        }
    }
}