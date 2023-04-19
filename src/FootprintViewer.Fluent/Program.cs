using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.Fluent.Extensions;
using FootprintViewer.Helpers;
using FootprintViewer.Logging;
using System.IO;
using System.Linq;

namespace FootprintViewer.Fluent;

public static class Program
{
    private static Global? Global;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        // Initialize the logger.
        string dataDir = EnvironmentHelpers.GetDataDir(Path.Combine("FootprintViewer", "Client"));
        SetupLogger(dataDir);

        Logger.LogDebug($"FootprintViewer was started with these argument(s): {(args.Any() ? string.Join(" ", args) : "none")}.");

        AppMode mode = AppMode.Release;// AppMode.DevWork;

        if (args.Length != 0)
        {
            if (Enum.TryParse(typeof(AppMode), args[0], true, out var res) == true)
            {
                mode = (AppMode)res!;
            }
        }

        var config = LoadOrCreateConfigs(dataDir);

        Global = CreateGlobal(config, mode);

        Services.Initialize(Global);

        AppBuilder
            .Configure(() => new App(async () => await Global.InitializeAsync()))
            .UseReactiveUI()
            .SetupAppBuilder()
            .StartWithClassicDesktopLifetime(args);
    }

    private static Config LoadOrCreateConfigs(string dataDir)
    {
        Directory.CreateDirectory(dataDir);

        Config config = new(Path.Combine(dataDir, "Config.json"));
        config.LoadFile(createIfMissing: true);

        return config;
    }

    private static Global CreateGlobal(Config config, AppMode mode)
    {
        return new Global(config, mode);
    }

    private static void SetupLogger(string dataDir)
    {
        LogLevel? logLevel = null;

        Logger.InitializeDefaults(Path.Combine(dataDir, "Logs.txt"), logLevel);
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder
        .Configure(() => new App())
        .UseReactiveUI()
        .SetupAppBuilder();
}
