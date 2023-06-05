using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PlannedScheduleOnMapSample.ViewModels;
using PlannedScheduleOnMapSample.Views;
using System;

namespace PlannedScheduleOnMapSample
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Mapsui.Logging.Logger.LogDelegate += (level, message, ex) =>
                   Console.WriteLine("Mapsui.Logging: " + level + " " + message + " " + ex);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = MainWindowViewModel.Instance,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}