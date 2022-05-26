using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
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
        private static GroundTargetViewer? _groundTargetViewer;

        public GroundTargetInfoView()
        {
            InitializeComponent();

            _enter = ReactiveCommand.Create(EnterImpl);

            _leave = ReactiveCommand.Create(LeaveImpl);

            this.WhenActivated(disposables =>
            {
                MainCard.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v._enter).DisposeWith(disposables);

                MainCard.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v._leave).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _enter;

        private readonly ReactiveCommand<Unit, Unit> _leave;

        private void EnterImpl()
        {
            _groundTargetViewer ??= Locator.Current.GetExistingService<GroundTargetViewer>();

            _groundTargetViewer?.ViewerList.MouseOverEnter.Execute(ViewModel!).Subscribe();
        }

        private void LeaveImpl()
        {
            _groundTargetViewer ??= Locator.Current.GetExistingService<GroundTargetViewer>();

            _groundTargetViewer?.ViewerList.MouseOverLeave.Execute().Subscribe();
        }
    }
}
