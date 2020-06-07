using ScriptingUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestLib;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using Vectors;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var testClasses = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(TestAttribute)));
            var benchmarks = (from t in testClasses
                              let benchs =
                                    (from m in t.GetMethods()
                                     let attribute = m.GetCustomAttributes<BenchmarkAttribute>().FirstOrDefault()
                                     where attribute != null
                                     select getBenchmark(m, attribute)).ToArray()
                              select new { t, benchs })
                              .ToDictionary(bm => bm.t, bm => bm.benchs);
                                   
            Console.WriteLine($"Observed: {testClasses.Count()} tests");
            foreach (var test in testClasses)
            {
                Console.WriteLine($"CURRENT TEST: {test.Name}");
                Activator.CreateInstance(test);

                var benchs = benchmarks[test];
                foreach (var bencmark in benchs)
                {
                    Console.WriteLine($"    BENCHMARK 1 OF {benchs.Count()}L:");
                    Console.WriteLine($"    {bencmark.Result.ToString()}");
                }

                Console.WriteLine($"TEST COMPLETE");
            }

            Console.WriteLine($"Press any key to exit...");
            Console.ReadKey();
        }

        class BenchmarkInfo
        {
            readonly Func<Info> _benchmark;
            Info _result;

            public string Name { get; }
            public string Description { get; }
            public int RepetitionsCount { get; }
            public Info Result
            {
                get => _result = _result ?? _benchmark();
            }

            public BenchmarkInfo(MethodInfo method, BenchmarkAttribute settings, Func<Info> benchmark)
            {
                Name = method.Name;
                Description = settings.Description;
                RepetitionsCount = settings.InnerLoopRepetitions * settings.Repetitions;
                _benchmark = benchmark;
            }
        }

        static BenchmarkInfo getBenchmark(MethodInfo method, BenchmarkAttribute settings)
        {
            var testMethod = (Action<object>)method.CreateDelegate(typeof(Action<object>));

            return new BenchmarkInfo(method, settings, doBenchmark);

            Info doBenchmark()
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < settings.Repetitions; i++)
                {
                    testMethod();
                }
                var elapsed = sw.Elapsed.TotalMilliseconds;
                return new Info(elapsed, elapsed / (settings.Repetitions * settings.InnerLoopRepetitions));
            }
        }

        class Info
        {
            public double ElapsedTotal { get; }
            public double ElapsedPerElement { get; }

            public Info(double elapsed, double elapsedPerElement)
            {
                ElapsedTotal = elapsed;
                ElapsedPerElement = elapsedPerElement;
            }

            public override string ToString()
            {
                return $"ElapsedPerElement: {ElapsedPerElement:P2}";
            }
        }
    }
}
