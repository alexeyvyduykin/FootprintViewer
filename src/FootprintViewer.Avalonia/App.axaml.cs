using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.Data;
using FootprintViewer.Layers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using Npgsql;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            if (Design.IsDesignMode == false)
            {
                LoadSettings();
            }
        }

        private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

        public override void OnFrameworkInitializationCompleted()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Create the AutoSuspendHelper.
            var suspension = new AutoSuspendHelper(ApplicationLifetime!);
            RxApp.SuspensionHost.CreateNewAppState = () => new AppSettings();
            RxApp.SuspensionHost.SetupDefaultSuspendResume(new Drivers.NewtonsoftJsonSuspensionDriver("_suspendAppSettings.json"));
            suspension.OnFrameworkInitializationCompleted();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                Bootstrapper.Register(Locator.CurrentMutable, Locator.Current);

                var mainViewModel = GetExistingService<MainViewModel>();

                if (mainViewModel != null)
                {
                    desktopLifetime.MainWindow = new Views.MainWindow()
                    {
                        DataContext = mainViewModel
                    };
                }
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime)
            {
                throw new Exception();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void LoadSettings()
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //// Create the AutoSuspendHelper.
            //var suspension = new AutoSuspendHelper(ApplicationLifetime!);
            //RxApp.SuspensionHost.CreateNewAppState = () => new MyLocalizationService();
            //RxApp.SuspensionHost.SetupDefaultSuspendResume(new Drivers.NewtonsoftJsonSuspensionDriver("suspendAppSettings.json"));
            //suspension.OnFrameworkInitializationCompleted();

            //var localizationService = GetRequiredService<ILocalizationService>();
            //var languageManager = GetRequiredService<ILanguageManager>();

            //languageManager.SetLanguage(localizationService.CurrentLanguage.Code);
        }

        public static async Task<string> OpenFileDialog(string? directory, string? filterName, string? filterExtension)
        {
            var settings = Locator.Current.GetService<AppSettings>()!;

            var dialog = new OpenFileDialog
            {
                Directory = directory ?? settings.LastOpenDirectory,
            };

            var ext = filterExtension ?? "*";

            dialog.Filters.Add(new FileDialogFilter() { Name = filterName, Extensions = { ext } });

            var result = await dialog.ShowAsync(GetWindow()!);

            if (result == null)
            {
                return string.Empty;
            }

            return result.First();
        }

        public static async Task<string> OpenFolderDialog(string? directory)
        {
            var settings = Locator.Current.GetService<AppSettings>()!;

            var dialog = new OpenFolderDialog()
            {
                Directory = directory ?? settings.LastOpenDirectory
            };

            var result = await dialog.ShowAsync(GetWindow()!);

            return result ?? string.Empty;
        }

        public static Window? GetWindow()
        {
            if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }

            return null;
        }
    }
}
