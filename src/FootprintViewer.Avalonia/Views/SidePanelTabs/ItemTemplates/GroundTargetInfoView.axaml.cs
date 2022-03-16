using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using Material.Styles;
using ReactiveUI;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class GroundTargetInfoView : ReactiveUserControl<GroundTargetInfo>
    {
        private Card MainCard => this.FindControl<Card>("MainCard");

        private TextBlock TypeTextBlock => this.FindControl<TextBlock>("TypeTextBlock");

        private TextBlock NameTextBlock => this.FindControl<TextBlock>("NameTextBlock");

        private static GroundTargetViewer? _groundTargetViewer;

        public GroundTargetInfoView()
        {
            InitializeComponent();

            _enter = ReactiveCommand.Create(EnterImpl);

            _leave = ReactiveCommand.Create(LeaveImpl);

            this.WhenActivated(disposables =>
            {
                this.MainCard.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v._enter).DisposeWith(disposables);
                
                this.MainCard.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v._leave).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Type, v => v.TypeTextBlock.Text, value => ((Data.GroundTargetType)value).ToString()).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameTextBlock.Text).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _enter;

        private readonly ReactiveCommand<Unit, Unit> _leave;

        private void EnterImpl()
        {
            _groundTargetViewer ??= Locator.Current.GetExistingService<GroundTargetViewer>();

            _groundTargetViewer?.ShowHighlight.Execute(ViewModel).Subscribe();
        }

        private void LeaveImpl()
        {
            _groundTargetViewer ??= Locator.Current.GetExistingService<GroundTargetViewer>();

            _groundTargetViewer?.HideHighlight.Execute().Subscribe();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
