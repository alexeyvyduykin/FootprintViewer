using FootprintViewer.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace FootprintViewer.Logging;

public static class Logger
{
    private static readonly object Lock = new();

    private static int LoggingFailedCount = 0;

    private static LogLevel MinimumLevel { get; set; } = LogLevel.Error;

    private static HashSet<LogMode> Modes { get; } = new HashSet<LogMode>();

    public static string FilePath { get; private set; } = "Log.txt";

    public static string EntrySeparator { get; private set; } = Environment.NewLine;

    // Default value is 10 MB
    private static long MaximumLogFileSize { get; set; } = 10_000;

    public static void InitializeDefaults(string filePath, LogLevel? logLevel = null)
    {
        SetFilePath(filePath);

#if RELEASE
			SetMinimumLevel(logLevel ??= LogLevel.Info);
			SetModes(LogMode.Console, LogMode.File);
#else
        SetMinimumLevel(logLevel ??= LogLevel.Debug);
        SetModes(LogMode.Debug, LogMode.Console, LogMode.File);
#endif

        MaximumLogFileSize = MinimumLevel == LogLevel.Trace ? 0 : 10_000;
    }

    public static void SetMinimumLevel(LogLevel level)
        => MinimumLevel = level;

    public static void SetModes(params LogMode[] modes)
    {
        if (Modes.Count != 0)
        {
            Modes.Clear();
        }

        if (modes is null)
        {
            return;
        }

        foreach (var mode in modes)
        {
            Modes.Add(mode);
        }
    }

    public static void SetFilePath(string filePath)
        => FilePath = filePath;

    public static void Log(LogLevel level, string message, int additionalEntrySeparators = 0, bool additionalEntrySeparatorsLogFileOnlyMode = true, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
    {
        try
        {
            if (Modes.Count == 0)
            {
                return;
            }

            if (level < MinimumLevel)
            {
                return;
            }

            var category = string.IsNullOrWhiteSpace(callerFilePath) ? "" : $"{EnvironmentHelpers.ExtractFileName(callerFilePath)}.{callerMemberName} ({callerLineNumber})";

            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"{DateTime.UtcNow.ToLocalTime():yyyy-MM-dd HH:mm:ss.fff} [{Environment.CurrentManagedThreadId}] {level.ToString().ToUpperInvariant()}\t");

            if (message.Length == 0)
            {
                if (category.Length == 0) // If both empty. It probably never happens though.
                {
                    messageBuilder.Append($"{EntrySeparator}");
                }
                else // If only the message is empty.
                {
                    messageBuilder.Append($"{category}{EntrySeparator}");
                }
            }
            else
            {
                if (category.Length == 0) // If only the category is empty.
                {
                    messageBuilder.Append($"{message}{EntrySeparator}");
                }
                else // If none of them empty.
                {
                    messageBuilder.Append($"{category}\t{message}{EntrySeparator}");
                }
            }

            var finalMessage = messageBuilder.ToString();

            for (int i = 0; i < additionalEntrySeparators; i++)
            {
                messageBuilder.Insert(0, EntrySeparator);
            }

            var finalFileMessage = messageBuilder.ToString();
            if (!additionalEntrySeparatorsLogFileOnlyMode)
            {
                finalMessage = finalFileMessage;
            }

            lock (Lock)
            {
                if (Modes.Contains(LogMode.Console))
                {
                    lock (Console.Out)
                    {
                        var color = Console.ForegroundColor;
                        switch (level)
                        {
                            case LogLevel.Warning:
                                color = ConsoleColor.Yellow;
                                break;

                            case LogLevel.Error:
                                color = ConsoleColor.Red;
                                break;

                            default:
                                break; // Keep original color.
                        }

                        Console.ForegroundColor = color;
                        Console.Write(finalMessage);
                        Console.ResetColor();
                    }
                }

                if (Modes.Contains(LogMode.Debug))
                {
                    Debug.Write(finalMessage);
                }

                if (!Modes.Contains(LogMode.File))
                {
                    return;
                }

                IoHelpers.EnsureContainingDirectoryExists(FilePath);

                if (MaximumLogFileSize > 0)
                {
                    if (File.Exists(FilePath))
                    {
                        var sizeInBytes = new FileInfo(FilePath).Length;
                        if (sizeInBytes > 1000 * MaximumLogFileSize)
                        {
                            File.Delete(FilePath);
                        }
                    }
                }

                File.AppendAllText(FilePath, finalFileMessage);
            }
        }
        catch (Exception ex)
        {
            if (Interlocked.Increment(ref LoggingFailedCount) == 1) // If it only failed the first time, try log the failure.
            {
                LogDebug($"Logging failed: {ex}");
            }

            // If logging the failure is successful then clear the failure counter.
            // If it's not the first time the logging failed, then we do not try to log logging failure, so clear the failure counter.
            Interlocked.Exchange(ref LoggingFailedCount, 0);
        }
    }

    private static void Log(Exception exception, LogLevel level, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
    {
        Log(level, exception.ToString(), callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);
    }

    public static void LogTrace(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(LogLevel.Trace, message, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogDebug(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(LogLevel.Debug, message, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogInfo(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(LogLevel.Info, message, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogWarning(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(LogLevel.Warning, message, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogWarning(Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(exception, LogLevel.Warning, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogError(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(LogLevel.Error, message, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);

    public static void LogError(Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = -1)
        => Log(exception, LogLevel.Error, callerFilePath: callerFilePath, callerMemberName: callerMemberName, callerLineNumber: callerLineNumber);
}