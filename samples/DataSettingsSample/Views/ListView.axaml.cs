using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;

namespace DataSettingsSample.Views
{
    public partial class ListView : ReactiveUserControl<ListViewModel>
    {
        public ListView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
