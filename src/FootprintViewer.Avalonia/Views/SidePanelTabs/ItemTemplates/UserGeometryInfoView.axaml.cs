using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class UserGeometryInfoView : ReactiveUserControl<UserGeometryInfo>
    {
        private TextBlock TypeTextBlock => this.FindControl<TextBlock>("TypeTextBlock");

        private TextBlock NameTextBlock => this.FindControl<TextBlock>("NameTextBlock");

        public UserGeometryInfoView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Type, v => v.TypeTextBlock.Text, value => ((Data.UserGeometryType)value).ToString()).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameTextBlock.Text).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
