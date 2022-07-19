using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class RandomSourceView : ReactiveUserControl<RandomSourceViewModel>
    {
        public RandomSourceView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
