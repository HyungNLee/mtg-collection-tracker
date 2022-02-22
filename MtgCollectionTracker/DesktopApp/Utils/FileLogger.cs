using System.IO;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace DesktopApp.Utils
{
    public class FileLogger
    {
        private static readonly string _defaultLogFileName = "log.txt";
        private static readonly LoggingLevelSwitch _logLevelSwitch = new();

        public static ILogger CreateFileLogger(LogEventLevel logEventLevel = LogEventLevel.Debug)
        {
            _logLevelSwitch.MinimumLevel = logEventLevel;

            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", _defaultLogFileName);

            Logger logConfig = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(_logLevelSwitch)
                .Enrich.FromLogContext()
                .WriteTo.File(new RenderedCompactJsonFormatter(),
                    logFilePath,
                    shared: true,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .CreateLogger();

            Log.Logger = logConfig;
            return logConfig;
        }
    }
}
