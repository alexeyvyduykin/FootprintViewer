﻿using Avalonia.Controls.ApplicationLifetimes;
using FootprintViewer.UI.ViewModels;
using FootprintViewer.UI.Views;

namespace FootprintViewer.UI;

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
            DataContext = MainViewModel.Instance
        };

        _lifetime.MainWindow = mainWindow;

        mainWindow.Show();
    }

    public void Shutdown()
    {
        throw new NotImplementedException();
    }
}
