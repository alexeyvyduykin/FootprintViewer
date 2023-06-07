using Avalonia.Controls;
using SpaceScienceTest.ViewModels;

namespace SpaceScienceTest.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = MainWindowViewModel.Instance;

        MapControl.Map = vm.Map;
    }
}