using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class GroundStationInfoView : ReactiveUserControl<GroundStationInfo>
    {
        private GroundStationViewer? _groundStationViewer;

        public GroundStationInfoView()
        {
            InitializeComponent();

            _update = ReactiveCommand.Create(UpdateImpl);

            this.WhenActivated(disposables =>
            {
                ViewModel.
                     WhenAnyValue(s => s.IsShow).
                     Select(args => Unit.Default).InvokeCommand(this, v => v._update).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _update;

        private void UpdateImpl()
        {
            _groundStationViewer ??= Locator.Current.GetExistingService<GroundStationViewer>();

            _groundStationViewer.Update(ViewModel!);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
