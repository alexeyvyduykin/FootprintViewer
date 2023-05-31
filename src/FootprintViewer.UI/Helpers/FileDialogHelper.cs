using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FootprintViewer.UI.Helpers;

public static class FileDialogHelper
{
    public static async Task<string?> ShowOpenFileDialogAsync(string title, string[] filterExtTypes, string? directory = null)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var defaultPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));

        var lastOpenDirectory = directory ?? defaultPath;

        var dialog = CreateOpenFileDialog(title, lastOpenDirectory);

        dialog.Filters = GenerateFilters(filterExtTypes);

        return await GetDialogAsync(dialog);
    }

    public static async Task<string?> ShowOpenFolderDialogAsync(string title, string? directory = null)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var defaultPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));

        var lastOpenDirectory = directory ?? defaultPath;

        var dialog = CreateOpenFolderDialog(title, lastOpenDirectory);

        return await GetDialogAsync(dialog);
    }

    private static async Task<string?> GetDialogAsync(OpenFileDialog dialog)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            && lifetime.MainWindow is { })
        {
            var result = await dialog.ShowAsync(lifetime.MainWindow);

            return result?.FirstOrDefault();
        }

        return null;
    }

    private static async Task<string?> GetDialogAsync(OpenFolderDialog dialog)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime
            && lifetime.MainWindow is { })
        {
            return await dialog.ShowAsync(lifetime.MainWindow);
        }

        return null;
    }

    private static OpenFileDialog CreateOpenFileDialog(string title, string directory)
    {
        return new OpenFileDialog
        {
            Title = title,
            Directory = directory,
        };
    }

    private static OpenFolderDialog CreateOpenFolderDialog(string title, string directory)
    {
        return new OpenFolderDialog()
        {
            Title = title,
            Directory = directory
        };
    }

    private static List<FileDialogFilter> GenerateFilters(string[] filterExtTypes)
    {
        var filters = new List<FileDialogFilter>();

        var generatedFilters =
            filterExtTypes
            .Where(s => s != "*")
            .Select(s =>
            new FileDialogFilter
            {
                Name = $"{s.ToUpper()} files",
                Extensions = new List<string> { s }
            });

        filters.AddRange(generatedFilters);

        if (filterExtTypes.Contains("*"))
        {
            filters.Add(new FileDialogFilter()
            {
                Name = "All files",
                Extensions = new List<string> { "*" }
            });
        }

        return filters;
    }
}
