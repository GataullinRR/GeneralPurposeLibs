using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Log4NetUtils
{
    public class Logger
    {
        readonly ILog _log;
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        string _indention = "";

        public Logger(Type declaringType)
        {
            FileInfo configFileInfo = new FileInfo("App.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(configFileInfo);

            _log = LogManager.GetLogger(declaringType);
        }
        //public Logger(ILog declaringType)
        //{
        //    _log = declaringType;
        //}

        public IDisposable BeginIndent()
        {
            _indention += '\t';
            return new DisposingAction(() => _indention = _indention.Remove(0, 1));
        }

        public void Debug(string message, [CallerMemberName] string callerMemberName = "")
        {
            log(_log.Debug, message, callerMemberName);
        }

        public void Warn(string message, [CallerMemberName] string callerMemberName = "")
        {
            log(_log.Warn, message, callerMemberName);
        }

        void log(Action<string> sender, string message, string callerMemberName)
        {
            lock (_stopwatch)
            {
                ThreadContext.Properties["TrueCName"] = callerMemberName;
                ThreadContext.Properties["DT"] = _stopwatch.Elapsed.TotalMilliseconds.Round().ToStringInvariant();
                _stopwatch.Restart();
            }
            sender(_indention + message);
        }

        //public static void Setup(string logsDirectory, string logFormat)
        //{
        //    IOUtils.CreateDirectoryIfNotExist(logsDirectory);
        //    Hierarchy hierarchy;
        //    try
        //    {
        //        hierarchy = (Hierarchy)LogManager.GetRepository(logsDirectory);
        //    }
        //    catch 
        //    {
        //        hierarchy = (Hierarchy)LogManager.CreateRepository(logsDirectory);
        //    }
            
        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = logFormat;// "%-5p %d{HH:mm:ss} %-5thread %5property{DT}ms %-25.25c{1} %-25.25property{TrueCName} - %m%n";
        //    patternLayout.ActivateOptions();

        //    RollingFileAppender roller = new RollingFileAppender();
        //    roller.AppendToFile = false;
        //    roller.File = @"\Log.txt";
        //    roller.Layout = patternLayout;
        //    roller.MaxSizeRollBackups = 10;
        //    roller.MaximumFileSize = "10MB";
        //    roller.RollingStyle = RollingFileAppender.RollingMode.Size;
        //    roller.StaticLogFileName = true;
        //    roller.ActivateOptions();
        //    hierarchy.Root.AddAppender(roller);

        //    //MemoryAppender memory = new MemoryAppender();
        //    //memory.ActivateOptions();
        //    //hierarchy.Root.AddAppender(memory);

        //    hierarchy.Root.Level = Level.Info;
        //    hierarchy.Configured = true;
        //}
    }
}
