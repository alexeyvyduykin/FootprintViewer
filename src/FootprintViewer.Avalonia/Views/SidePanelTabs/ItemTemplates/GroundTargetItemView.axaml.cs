using Avalonia.Controls;
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
    public partial class GroundTargetItemView : ReactiveUserControl<GroundTargetViewModel>
    {
        private static GroundTargetTab? _groundTargetTab;

        public GroundTargetItemView()
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
            _groundTargetTab ??= Locator.Current.GetExistingService<GroundTargetTab>();

            _groundTargetTab?.ViewerList.MouseOverEnter.Execute(ViewModel!).Subscribe();
        }

        private void LeaveImpl()
        {
            _groundTargetTab ??= Locator.Current.GetExistingService<GroundTargetTab>();

            _groundTargetTab?.ViewerList.MouseOverLeave.Execute().Subscribe();
        }
    }
}
