using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using controls = FootprintViewer.Avalonia.Controls;

namespace FootprintViewer.Avalonia.Views
{
    public partial class ToolBarView : ReactiveUserControl<ToolBar>
    {
        private controls.Flyout Flyout => this.FindControl<controls.Flyout>("FlyoutToolBar");
        
        private Button LayerSelectorButton => this.FindControl<Button>("LayerSelectorButton");
        
        public ToolBarView()
        {
            InitializeComponent();

          //  _click = ReactiveCommand.Create(ClickImpl);

            this.WhenActivated(disposables =>
            {
                // LayerSelectorButton               
             //   this.LayerSelectorButton.Events().Click.Select(args => Unit.Default).InvokeCommand(this, v => v._click).DisposeWith(disposables);
            });
        }

       // private readonly ReactiveCommand<Unit, Unit> _click;

        private void ClickImpl()
        {
            //Flyout.Click();

         //   Flyout.IsOpen = !Flyout.IsOpen;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
