using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.AppStates;
using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Text;

namespace FootprintViewer.Avalonia;

public class App : Application
{
    private static Views.MainWindow? _mainWindow;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        if (Design.IsDesignMode == false)
        {
            CreateAutoSuspendHelper();
            LoadSettings();
        }
    }

    private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var mainViewModel = GetExistingService<MainViewModel>();

            if (mainViewModel != null)
            {
                _mainWindow = new Views.MainWindow()
                {
                    DataContext = mainViewModel
                };

                desktopLifetime.MainWindow = _mainWindow;
            }
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
        //var settings = GetExistingService<SettingsViewModel>(); 
        var dataManager = GetExistingService<IDataManager>();

        //settings.LanguageSettings?.LoadState(mainState.LocalizationState);

        mainState.InitDefaultData(dataManager);

        mainState.LoadData(dataManager);

        dataManager.UpdateData();

        Locator.CurrentMutable.RegisterConstant(mainState, typeof(MainState));
    }

    public static Views.MainWindow GetMainWindow() => _mainWindow ?? throw new Exception();
}
