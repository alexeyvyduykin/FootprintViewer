using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DataSettingsSample.ViewModels;
using DataSettingsSample.Views;
using Splat;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DataSettingsSample
{
    public partial class App : Application
    {
        private static string? _lastOpenDirectory;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainViewModel = GetExistingService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static T GetExistingService<T>() => Locator.Current.GetExistingService<T>();

        public static async Task<string> OpenFolderDialog(string? directory)
        {
            var dialog = new OpenFolderDialog()
            {
                Directory = directory ?? (_lastOpenDirectory ??= GetRoot())
            };

            var result = await dialog.ShowAsync(GetWindow()!);

            _lastOpenDirectory = result;

            return result ?? string.Empty;
        }

        private static string GetRoot()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            return Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets"));
        }

        private static Window? GetWindow()
        {
            if (Application.Current != null && Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }

            return null;
        }
    }
}