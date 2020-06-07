using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace TestConsole
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false, Inherited = false)]
    class BenchmarkAttribute : Attribute
    {
        public string Description;
        public int Repetitions;
        public int InnerLoopRepetitions;

        public BenchmarkAttribute(int repetitions, int innerLoopRepetitions, string description)
        {
            Repetitions = repetitions;
            InnerLoopRepetitions = innerLoopRepetitions;
            Description = description;
        }

        public BenchmarkAttribute(int repetitions, int innerLoopRepetitions) 
            : this(repetitions, innerLoopRepetitions, "") { }

        public BenchmarkAttribute(int repetitions)
            : this(repetitions, 1, "") { }
    }
}
