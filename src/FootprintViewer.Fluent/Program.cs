using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.Helpers;
using FootprintViewer.Logging;
using System.IO;
using System.Linq;

namespace FootprintViewer.Fluent;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
        // Initialize the logger.
        string dataDir = EnvironmentHelpers.GetDataDir(Path.Combine("FootprintViewer", "Client"));
        SetupLogger(dataDir);

        Logger.LogDebug($"FootprintViewer was started with these argument(s): {(args.Any() ? string.Join(" ", args) : "none")}.");

        AppMode mode = AppMode.DevWork;// Release;

        if (args.Length != 0)
        {
            if (Enum.TryParse(typeof(AppMode), args[0], true, out var res) == true)
            {
                mode = (AppMode)res!;
            }
        }

        RegisterBootstrapper(mode);

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static void RegisterBootstrapper(AppMode mode)
    {
        Bootstrapper.Register(Splat.Locator.CurrentMutable, Splat.Locator.Current, mode);
    }

    private static void SetupLogger(string dataDir)
    {
        LogLevel? logLevel = null;

        Logger.InitializeDefaults(Path.Combine(dataDir, "Logs.txt"), logLevel);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}
