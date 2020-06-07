using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace TestConsole.Tests
{
    [Test]
    class UniversalMathPerformance
    {
        List<float> array1 = Global.Random.GenerateSingles(1000);
        List<float> array2 = Global.Random.GenerateSingles(1000);
        public UniversalMathPerformance()
        {

        }

        [Benchmark(10, 1000)]
        public void test()
        {
            for (int i = 0; i < 1000; i++)
            {
                array1.Sum(array2);
            }
        }
    }
}
