using FootprintViewer.Extensions;
using FootprintViewer.Logging;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;

namespace FootprintViewer.Helpers;

public static class EnvironmentHelpers
{
    // appName, dataDir
    private static ConcurrentDictionary<string, string> DataDirDict { get; } = new ConcurrentDictionary<string, string>();

    public static string GetDataDir(string appName)
    {
        if (DataDirDict.TryGetValue(appName, out string? dataDir))
        {
            return dataDir;
        }

        string directory;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == false)
        {
            var home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                directory = Path.Combine(home, "." + appName.ToLowerInvariant());
                Logger.LogInfo($"Using HOME environment variable for initializing application data at `{directory}`.");
            }
            else
            {
                throw new DirectoryNotFoundException("Could not find suitable datadir.");
            }
        }
        else
        {
            var localAppData = Environment.GetEnvironmentVariable("APPDATA");
            if (!string.IsNullOrEmpty(localAppData))
            {
                directory = Path.Combine(localAppData, appName);
                Logger.LogInfo($"Using APPDATA environment variable for initializing application data at `{directory}`.");
            }
            else
            {
                throw new DirectoryNotFoundException("Could not find suitable datadir.");
            }
        }

        if (Directory.Exists(directory))
        {
            DataDirDict.TryAdd(appName, directory);
            return directory;
        }

        Logger.LogInfo($"Creating data directory at `{directory}`.");
        Directory.CreateDirectory(directory);

        DataDirDict.TryAdd(appName, directory);
        return directory;
    }

    // This method removes the path and file extension.
    public static string ExtractFileName(string callerFilePath)
    {
        var lastSeparatorIndex = callerFilePath.LastIndexOf("\\");
        if (lastSeparatorIndex == -1)
        {
            lastSeparatorIndex = callerFilePath.LastIndexOf("/");
        }

        var fileName = callerFilePath;

        if (lastSeparatorIndex != -1)
        {
            lastSeparatorIndex++;
            fileName = callerFilePath[lastSeparatorIndex..]; // From lastSeparatorIndex until the end of the string.
        }

        var fileNameWithoutExtension = fileName.TrimEnd(".cs", StringComparison.InvariantCultureIgnoreCase);
        return fileNameWithoutExtension;
    }

    public static string GetFullBaseDirectory()
    {
        var fullBaseDirectory = Path.GetFullPath(AppContext.BaseDirectory);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!fullBaseDirectory.StartsWith('/'))
            {
                fullBaseDirectory = fullBaseDirectory.Insert(0, "/");
            }
        }

        return fullBaseDirectory;
    }
}
