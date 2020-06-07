using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vectors;

namespace VectorsTests
{
    [TestClass]
    public class EdgeTests
    {
        [TestMethod]
        public void GetOuterNormal_Test()
        {
            var edge = new Edge(V2.Zero, V2.One);
            var normal = edge.GetOuterNormal(new V2(0, 1));

            var desired = new V2(0.7071, -0.7071);
            var actual = normal.Round(4);

            Assert.AreEqual(desired, actual);
        }
    }
}
