using Avalonia.ReactiveUI;
using DataSettingsSample.ViewModels;
using ReactiveUI;

namespace DataSettingsSample.Views.SourceBuilders
{
    public partial class JsonBuilderView : ReactiveUserControl<JsonBuilderViewModel>
    {
        public JsonBuilderView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {

            });
        }
    }
}
