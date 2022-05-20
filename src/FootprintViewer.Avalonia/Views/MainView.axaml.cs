using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Views
{
    public partial class MainView : ReactiveUserControl<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();
        }
    }
}
