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

        private StackPanel CollectionStackPanel => this.FindControl<StackPanel>("CollectionStackPanel");
        
        public ToolBarView()
        {
            InitializeComponent();
                     
            _enter = ReactiveCommand.Create(EnterImpl);
            _leave = ReactiveCommand.Create(LeaveImpl);

            this.WhenActivated(disposables =>
            {
                CollectionStackPanel.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v._enter).DisposeWith(disposables);
                
                CollectionStackPanel.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v._leave).DisposeWith(disposables);          
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _enter;

        private readonly ReactiveCommand<Unit, Unit> _leave;
     
        private void EnterImpl()
        {
            if (ViewModel != null)
            {
                ViewModel.AOICollection.Visible = true;
            }
        }

        private void LeaveImpl()
        {
            if (ViewModel != null)
            {
                ViewModel.AOICollection.Visible = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
