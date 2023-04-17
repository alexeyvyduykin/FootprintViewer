using System.IO;

namespace FootprintViewer.Helpers;

public static class IoHelpers
{
    public static void EnsureContainingDirectoryExists(string fileNameOrPath)
    {
        string fullPath = Path.GetFullPath(fileNameOrPath); // No matter if relative or absolute path is given to this.

        string? dir = Path.GetDirectoryName(fullPath);

        if (string.IsNullOrWhiteSpace(dir) == false) // If root is given, then do not worry.
        {
            Directory.CreateDirectory(dir); // It does not fail if it exists.
        }
    }
}
