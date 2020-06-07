using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Logging
{
    public enum LogType
    {
        OK,
        INFO,
        ERROR,
        WARNING
    }

    public class Logger
    {
        public const string TIME_FORMAT = "HH:mm:ss.fff";

        readonly StreamWriter _storage;

        int _indentLevel = 0;
        readonly ConcurrentQueue<string> _linesToWrite = new ConcurrentQueue<string>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable IndentMode
        {
            get
            {
                Interlocked.Increment(ref _indentLevel);
                return new DisposingAction(() => Interlocked.Decrement(ref _indentLevel));
            }
        }

        public Logger(StreamWriter storage)
        {
            try
            {
                _storage = storage;

                writeFileLoopAsync();
            }
            catch
            {

            }

            async void writeFileLoopAsync()
            {
                while (true)
                {
                    while (_linesToWrite.Count > 0)
                    {
                        var hasLine = _linesToWrite.TryDequeue(out string line);
                        if (hasLine)
                        {
                            _storage.WriteLine(line);
                        }
                    }
                    _storage.Flush();

                    await Task.Delay(10);
                }
            }
        }

        public void LogIf(bool condition, string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            if (condition)
            {
                Log(text, caller, callerFilePath);
            }
        }
        public void LogWarningIf(bool condition, string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            if (condition)
            {
                Log(text, LogType.WARNING, caller, callerFilePath);
            }
        }
        public void LogErrorIf(bool condition, string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            if (condition)
            {
                Log(text, LogType.ERROR, caller, callerFilePath);
            }
        }
        public void Log(string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            Log(text, LogType.INFO, caller, callerFilePath);
        }
        public void LogError(string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            Log(text, LogType.ERROR, caller, callerFilePath);
        }
        public void LogError(string text, Exception ex, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            Log(getMessage(), LogType.ERROR, caller, callerFilePath);

            string getMessage()
            {
                try
                {
                    return ex == null
                        ? text
                        : $"{text}{getExceptionInfo()}";
                }
                catch
                {
                    return $"LOGGER ERROR: Error generating LogError message. For caller:{caller}, ex:{ex}, text:{text}";
                }
            }

            string getExceptionInfo()
            {
                var e = ex;
                var sb = new StringBuilder();
                int depth = 0;
                while (e != null)
                {
                    var newLine = $"{Global.NL}{new string('-', depth * 5)}";
                    sb.Append($"{newLine}{(depth == 0).Ternar("An exception was caught:", "Inner exception:")} Type: {ex.GetType()}{newLine}Message: {ex.Message}{newLine}StackTrace: {ex.StackTrace}");
                    e = e.InnerException;
                    depth++;
                }

                return sb.ToString();
            }
        }
        public void LogWarning(string text, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            Log(text, LogType.WARNING, caller, callerFilePath);
        }

        public void Log(string text, LogType logType, [CallerMemberName]string caller = "", [CallerFilePath]string callerFilePath = "")
        {
            try
            {
                var time = DateTime.Now;
                var preliminary = $"{time.ToString(TIME_FORMAT)}-{"---".Repeat(_indentLevel)}{toString(logType)}> ";
                text = text.Replace(Global.NL, Global.NL + " ".Repeat(preliminary.Length));
                _linesToWrite.Enqueue($"{preliminary}{Path.GetFileName(callerFilePath)}\\{caller}: {text}");
            }
            catch
            {
                Debug.Print("There was an error in logger.");
                Debugger.Break();
            }

            string toString(LogType lt)
            {
                switch (lt)
                {
                    case LogType.OK:
                        return "OK!";
                    case LogType.INFO:
                        return "INF";
                    case LogType.ERROR:
                        return "ERR";
                    case LogType.WARNING:
                        return "WRN";

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
