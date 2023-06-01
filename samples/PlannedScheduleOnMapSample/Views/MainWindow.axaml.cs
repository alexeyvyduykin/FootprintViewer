using Avalonia.Controls;
using Mapsui.UI.Avalonia;
using PlannedScheduleOnMapSample.ViewModels;
using System;

namespace PlannedScheduleOnMapSample.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel viewModel)
        {
            MapControl.Map = viewModel.Map;
        }
    }
}