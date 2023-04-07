using Avalonia;
using Avalonia.ReactiveUI;
using Splat;

namespace FootprintViewer.Fluent;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static void Main(string[] args)
    {
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
        // I only want to hear about errors
        var logger = new ConsoleLogger() { Level = Splat.LogLevel.Error };
        Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));

        Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, mode);
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
