using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.AppStates;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia
{
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

        private void LoadSettings()
        {
            var mainState = RxApp.SuspensionHost.GetAppState<MainState>();

            Locator.CurrentMutable.RegisterConstant(mainState, typeof(MainState));

            var settings = GetExistingService<SettingsTabViewModel>();

            settings.LanguageSettings?.LoadState(mainState.LocalizationState);

            settings.Find(Data.ProviderType.MapBackgrounds)?.LoadState(mainState.MapBackgroundProvider);
            settings.Find(Data.ProviderType.FootprintPreviews)?.LoadState(mainState.FootprintPreviewProvider);
            settings.Find(Data.ProviderType.Satellites)?.LoadState(mainState.SatelliteProvider);
            settings.Find(Data.ProviderType.GroundStations)?.LoadState(mainState.GroundStationProvider);
            settings.Find(Data.ProviderType.GroundTargets)?.LoadState(mainState.GroundTargetProvider);
            settings.Find(Data.ProviderType.Footprints)?.LoadState(mainState.FootprintProvider);
            settings.Find(Data.ProviderType.UserGeometries)?.LoadState(mainState.UserGeometryProvider);
        }

        public static async Task<string> OpenFileDialog(string? directory, string? filterName, string? filterExtension)
        {
            var mainState = Locator.Current.GetService<MainState>();

            string? lastOpenDirectory = (mainState != null) ? mainState.LastOpenDirectory : string.Empty;

            var dialog = new OpenFileDialog
            {
                Directory = directory ?? lastOpenDirectory,
            };

            var ext = filterExtension ?? "*";

            dialog.Filters.Add(new FileDialogFilter() { Name = filterName, Extensions = { ext } });

            var result = await dialog.ShowAsync(GetWindow()!);

            if (result == null)
            {
                return string.Empty;
            }

            if (mainState != null)
            {
                mainState.LastOpenDirectory = System.IO.Path.GetDirectoryName(result.First());
            }

            return result.First();
        }

        public static async Task<string> OpenFolderDialog(string? directory)
        {
            var mainState = Locator.Current.GetService<MainState>();

            string? lastOpenDirectory = (mainState != null) ? mainState.LastOpenDirectory : string.Empty;

            var dialog = new OpenFolderDialog()
            {
                Directory = directory ?? lastOpenDirectory
            };

            var result = await dialog.ShowAsync(GetWindow()!);

            if (mainState != null)
            {
                mainState.LastOpenDirectory = result;
            }

            return result ?? string.Empty;
        }

        private static Window? GetWindow()
        {
            if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }

            return null;
        }

        public static Views.MainWindow GetMainWindow() => _mainWindow ?? throw new Exception();
    }
}
