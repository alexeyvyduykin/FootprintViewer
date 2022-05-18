using Avalonia.Media;
using Avalonia.ReactiveUI;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Disposables;
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
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.RectangleBorder.Background, ConvertToBrush).DisposeWith(disposables);

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

        private static IBrush ConvertToBrush(string? name)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                return new SolidColorBrush() { Color = Colors.Black };
            }

            var color = LayerStyleManager.SatellitePalette.PickColor(name);

            return new SolidColorBrush()
            {
                Color = Color.FromRgb(color.R, color.G, color.B)
            };
        }
    }
}
