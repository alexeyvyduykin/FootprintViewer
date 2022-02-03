using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.Models;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class ToolCheckView : ReactiveUserControl<IToolCheck>
    {
        private ToggleButton ToggleButton => this.FindControl<ToggleButton>("ToggleButton");

        public ToolCheckView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                ToggleButton.Events().Click.Select(args => ToggleButton.IsChecked).InvokeCommand(this, v => v.ViewModel!.Check).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
