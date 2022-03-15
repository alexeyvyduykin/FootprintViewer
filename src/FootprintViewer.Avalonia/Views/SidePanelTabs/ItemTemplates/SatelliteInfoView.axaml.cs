using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class SatelliteInfoView : ReactiveUserControl<SatelliteInfo>
    {
        private SatelliteViewer? _satelliteViewer;

        public SatelliteInfoView()
        {
            InitializeComponent();

            _updateTrack = ReactiveCommand.Create(UpdateTrackImpl);

            _updateStrips = ReactiveCommand.Create(UpdateStripsImpl);

            this.WhenActivated(disposables =>
            {
                ViewModel.
                     WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsTrack).
                     Select(args => Unit.Default).InvokeCommand(this, v => v._updateTrack);

                ViewModel.
                     WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsLeftStrip, s => s.IsRightStrip).
                     Select(args => Unit.Default).InvokeCommand(this, v => v._updateStrips);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _updateTrack;

        private readonly ReactiveCommand<Unit, Unit> _updateStrips;

        private void UpdateTrackImpl()
        {
            _satelliteViewer ??= Locator.Current.GetExistingService<SatelliteViewer>();

            _satelliteViewer.UpdateTrack(ViewModel!);
        }

        private void UpdateStripsImpl()
        {
            _satelliteViewer ??= Locator.Current.GetExistingService<SatelliteViewer>();

            _satelliteViewer.UpdateStrips(ViewModel!);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
