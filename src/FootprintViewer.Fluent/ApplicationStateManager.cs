using Avalonia.Controls.ApplicationLifetimes;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Fluent.Views;
using Splat;

namespace FootprintViewer.Fluent;

public class ApplicationStateManager : IMainWindowService
{
    private readonly IClassicDesktopStyleApplicationLifetime _lifetime;

    internal ApplicationStateManager(IClassicDesktopStyleApplicationLifetime lifetime)
    {
        _lifetime = lifetime;

        ApplicationViewModel = new ApplicationViewModel(this);

        CreateAndShowMainWindow();
    }

    internal ApplicationViewModel ApplicationViewModel { get; }

    private void CreateAndShowMainWindow()
    {
        if (_lifetime.MainWindow is { })
        {
            return;
        }

        var mainWindow = new MainWindow
        {
            //DataContext = MainViewModel.Instance
            DataContext = Locator.Current.GetExistingService<MainViewModel>()
        };

        _lifetime.MainWindow = mainWindow;

        mainWindow.Show();
    }

    public void Shutdown()
    {
        throw new NotImplementedException();
    }
}
