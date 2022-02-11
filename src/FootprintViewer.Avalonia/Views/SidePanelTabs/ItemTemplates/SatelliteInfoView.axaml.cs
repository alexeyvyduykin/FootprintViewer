using Avalonia;
using Avalonia.Controls;
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
    public partial class SatelliteInfoView : ReactiveUserControl<SatelliteInfo>
    {
        private CheckBox ShowCheckBox => this.FindControl<CheckBox>("ShowCheckBox");

        private Button ShowInfoButton => this.FindControl<Button>("ShowInfoButton");

        private CheckBox TrackCheckBox => this.FindControl<CheckBox>("TrackCheckBox");

        private CheckBox LeftStripCheckBox => this.FindControl<CheckBox>("LeftStripCheckBox");

        private CheckBox RightStripCheckBox => this.FindControl<CheckBox>("RightStripCheckBox");

        private StackPanel InfoStackPanel => this.FindControl<StackPanel>("InfoStackPanel");

        private Slider Slider => this.FindControl<Slider>("Slider");

        private SatelliteViewer? _satelliteViewer;

        public SatelliteInfoView()
        {
            InitializeComponent();

            _updateTrack = ReactiveCommand.Create(UpdateTrackImpl);

            _updateStrips = ReactiveCommand.Create(UpdateStripsImpl);

            this.WhenActivated(disposables =>
            {
                // ShowCheckBox
                this.Bind(ViewModel, vm => vm.IsShow, v => v.ShowCheckBox.IsChecked).DisposeWith(disposables);

                // ShowInfoButton
                ShowInfoButton.Events().Click.Select(args => Unit.Default).InvokeCommand(ViewModel, vm => vm.ShowInfoClick).DisposeWith(disposables);

                // TrackCheckBox
                this.Bind(ViewModel, vm => vm.IsTrack, v => v.TrackCheckBox.IsChecked).DisposeWith(disposables);

                // LeftStripCheckBox
                this.Bind(ViewModel, vm => vm.IsLeftStrip, v => v.LeftStripCheckBox.IsChecked).DisposeWith(disposables);

                // RightStripCheckBox
                this.Bind(ViewModel, vm => vm.IsRightStrip, v => v.RightStripCheckBox.IsChecked).DisposeWith(disposables);

                // InfoStackPanel
                this.OneWayBind(ViewModel, vm => vm.IsShowInfo, v => v.InfoStackPanel.IsVisible).DisposeWith(disposables);

                // Slider
                this.OneWayBind(ViewModel, vm => vm.MinNode, v => v.Slider.Minimum).DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.MaxNode, v => v.Slider.Maximum).DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.CurrentNode, v => v.Slider.Value).DisposeWith(disposables);

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
