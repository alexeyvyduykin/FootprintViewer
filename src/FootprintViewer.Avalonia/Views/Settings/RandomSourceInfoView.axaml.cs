using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;

namespace FootprintViewer.Avalonia.Views.Settings
{
    public partial class RandomSourceInfoView : ReactiveUserControl<RandomSourceInfo>
    {
        public RandomSourceInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
