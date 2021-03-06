using System;

namespace FootprintViewer.Avalonia
{
    public enum LogLevel
    {
        Error,
        Warning,
        Information,
        Debug,
        Trace
    }

    public static class Logger
    {
        public static Action<LogLevel, string, Exception?>? LogDelegate
        {
            get;
            set;
        }

        public static void Log(LogLevel level, string message, Exception? exception = null)
        {
            LogDelegate?.Invoke(level, message, exception);
        }
    }
}
