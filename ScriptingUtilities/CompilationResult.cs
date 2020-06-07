using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace ScriptingUtils
{
    public class CompilationResult<TScript>
    {
        public TScript Script { get; private set; }
        public List<string> Errors { get; private set; }
        public bool Success
        {
            get => Errors.Count == 0;
        }

        public CompilationResult(TScript script, List<string> errors)
        {
            Errors = errors;
            Script = script;
        }
    }
}
