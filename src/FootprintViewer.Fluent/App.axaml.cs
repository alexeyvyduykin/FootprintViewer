using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.AppStates;
using FootprintViewer.Fluent.ViewModels;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent;

public class App : Application
{
    private readonly Func<Task>? _backendInitialiseAsync;
    private ApplicationStateManager? _applicationStateManager;

    public App()
    {
        Name = "FootprintViewer";
    }

    public App(Func<Task> backendInitialiseAsync) : this()
    {
        _backendInitialiseAsync = backendInitialiseAsync;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode == false)
        {
            CreateAutoSuspendHelper();
            LoadSettings();
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            _applicationStateManager = new ApplicationStateManager(desktopLifetime);

            DataContext = _applicationStateManager.ApplicationViewModel;

            RxApp.MainThreadScheduler.Schedule(
                async () =>
                {
                    await _backendInitialiseAsync!(); // Guaranteed not to be null when not in designer.

                    MainViewModel.Instance.Initialize();

                    await MainViewModel.Instance.InitAsync();
                });
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            throw new Exception();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void CreateAutoSuspendHelper()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Create the AutoSuspendHelper.
        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new MainState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new Drivers.NewtonsoftJsonSuspensionDriver("appstate.json"));
        suspension.OnFrameworkInitializationCompleted();
    }

    private static void LoadSettings()
    {
        var mainState = RxApp.SuspensionHost.GetAppState<MainState>();

        Services.MainState = mainState;
    }
}
