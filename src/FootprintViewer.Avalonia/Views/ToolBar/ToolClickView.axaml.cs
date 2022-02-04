using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.Models;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.ToolBar
{
    public partial class ToolClickView : ReactiveUserControl<IToolClick>
    {
        private Button Button => this.FindControl<Button>("Button");

        public ToolClickView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                Button.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v.ViewModel!.Click).DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}