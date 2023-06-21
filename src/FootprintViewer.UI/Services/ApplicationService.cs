using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FootprintViewer.UI.Services2;

public class ApplicationService
{
    private static FilePickerFileType Json { get; } = new("Json")
    {
        Patterns = new[] { "*.json" },
        AppleUniformTypeIdentifiers = new[] { "public.json" },
        MimeTypes = new[] { "application/json" }
    };

    private static FilePickerFileType Mbtiles { get; } = new("Mbtiles")
    {
        Patterns = new[] { "*.mbtiles" },
        AppleUniformTypeIdentifiers = new[] { "public.mbtiles" },
        MimeTypes = new[] { "application/mbtiles" }
    };

    public async Task<string?> ShowOpenFileDialogAsync(string title, string[] filterExtTypes, string? directory = null)
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var defaultPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));

        var lastOpenDirectory = directory ?? defaultPath;

        var storageProvider = GetStorageProvider();

        if (storageProvider is null)
        {
            return null;
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            // SuggestedStartLocation = storageProvider.TryGetWellKnownFolderAsync(),
            FileTypeFilter = GenerateFilters(filterExtTypes),
            AllowMultiple = false
        });

        var file = result.FirstOrDefault();

        if (file != null)
        {
            return file.Path.LocalPath;
        }

        return null;
    }

    private static List<FilePickerFileType> GenerateFilters(string[] filters)
    {
        var types = new List<FilePickerFileType>();

        foreach (var item in filters)
        {
            if (item == "Json")
            {
                types.Add(Json);
            }
            else if (item == "Mbtiles")
            {
                types.Add(Mbtiles);
            }
        }

        return types;
    }

    private static IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is { } window)
            {
                return window.StorageProvider;
            }
        }

        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime view)
        {
            if (view.MainView is { } mainView)
            {
                var visualRoot = mainView.GetVisualRoot();
                if (visualRoot is TopLevel topLevel)
                {
                    return topLevel.StorageProvider;
                }
            }
        }

        return null;
    }

    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}