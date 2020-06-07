using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace ScriptingUtils
{
    public class ScriptCompiller<TScript>
            where TScript : class
    {
        static ScriptCompiller()
        {
            Assembly a = Assembly.Load("system, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            IncludedAssembliesPublicTypes = a.GetTypes().Where(t => t.IsPublic).Select(t => t.Name).ToArray();
        }

        public static string[] IncludedAssembliesPublicTypes;

        static readonly string[] Assemblies =
        {
            "System.dll",
            //"System.Collections.Generic.dll",
            //"System.Text.dll",
            "System.Windows.Forms.dll",
            "System.Drawing.dll",
            "System.Linq.dll",
            "System.IO.dll",
            "System.Threading.dll"
        };

        public CompilationResult<TScript> Compile(string sourceCode)
        {
            return Compile(sourceCode, null);
        }
        public CompilationResult<TScript> Compile(string sourceCode, params string[] additionalAssemblies)
        {
            CSharpCodeProvider sharpProvider = new CSharpCodeProvider();
            CompilerParameters compilerParams = getCompillerParams(additionalAssemblies);
            string script = addStandartUsings(sourceCode);
            CompilerResults compiled = sharpProvider.CompileAssemblyFromSource(compilerParams, script);

            List<string> errors = aggregateErrorsInformation(compiled.Errors);
            TScript scriptInstance = instatiateScriptType(compiled);
            return new CompilationResult<TScript>(scriptInstance, errors);
        }

        string addStandartUsings(string sourceCode)
        {
            string usings = "using System; using System.Threading; using System.IO;";
            return usings + sourceCode;
        }

        static CompilerParameters getCompillerParams(params string[] additionalAssemblies)
        {
            var assemblies = new List<string>(Assemblies);
            if (additionalAssemblies != null)
                assemblies.AddRange(additionalAssemblies);

            var compilerParams = new CompilerParameters();
            compilerParams = new CompilerParameters();
            compilerParams.ReferencedAssemblies.AddRange(assemblies.ToArray());
            compilerParams.GenerateInMemory = true;
            compilerParams.GenerateExecutable = false;
            return compilerParams;
        }

        static TScript instatiateScriptType(CompilerResults result)
        {
            try
            {
                if (result != null && result.CompiledAssembly != null)
                {
                    Type script = null;
                    foreach (var t in result.CompiledAssembly.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(TScript)) && !t.IsAbstract)
                        {
                            script = t;
                            break;
                        }
                    }

                    return script == null ? null : (TScript)Activator.CreateInstance(script);
                }
            }
            catch { }

            return null;
        }

        static List<string> aggregateErrorsInformation(CompilerErrorCollection compilerErrors)
        {
            List<string> errors = new List<string>();
            foreach (CompilerError error in compilerErrors)
            {
                string errorMsg = string.Format("Line: {0} Error {1} mesage {2}",
                    error.Line, error.ErrorNumber, error.ErrorText);
                errors.Add(errorMsg);
            }

            return errors;
        }
    }
}
