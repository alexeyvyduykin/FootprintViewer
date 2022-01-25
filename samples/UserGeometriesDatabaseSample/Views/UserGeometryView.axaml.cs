using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UserGeometriesDatabaseSample.Data;

namespace UserGeometriesDatabaseSample.Views
{
    public partial class UserGeometryView : ReactiveUserControl<UserGeometry>
    {
        private TextBlock UserGeometryTextBlock => this.FindControl<TextBlock>("UserGeometryTextBlock");

        public UserGeometryView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.UserGeometryTextBlock.Text, value => Convert()).DisposeWith(disposables);
            });
        }

        private object Convert()
        {
            return $"{ViewModel?.Name} (type: {ViewModel?.Type})";
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
