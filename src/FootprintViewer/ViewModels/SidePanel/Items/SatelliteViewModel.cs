using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SatelliteViewModel : ReactiveObject, IViewerItem
    {
        private readonly string _name;
        private readonly Satellite _satellite;
        private readonly int _minNode = 1;
        private readonly int _maxNode;

        public SatelliteViewModel(Satellite satellite)
        {
            _satellite = satellite;

            _name = satellite.Name!;

            var count = satellite.ToPRDCTSatellite().Nodes().Count;

            _maxNode = count;

            CurrentNode = 1;

            ShowInfoClick = ReactiveCommand.Create(ShowInfoClickImpl);
        }

        public IObservable<SatelliteViewModel> TrackObservable => 
            this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsTrack).Select(_ => this);

        public IObservable<SatelliteViewModel> StripsObservable => 
            this.WhenAnyValue(s => s.IsShow, s => s.CurrentNode, s => s.IsLeftStrip, s => s.IsRightStrip).Select(_ => this);

        private void ShowInfoClickImpl() => IsShowInfo = !IsShowInfo;

        public ReactiveCommand<Unit, Unit> ShowInfoClick { get; }

        public string Name => _name;

        public Satellite Satellite => _satellite;

        public int MinNode => _minNode;

        public int MaxNode => _maxNode;

        [Reactive]
        public int CurrentNode { get; set; }

        [Reactive]
        public bool IsShow { get; set; } = false;

        [Reactive]
        public bool IsShowInfo { get; set; } = false;

        [Reactive]
        public bool IsTrack { get; set; } = true;

        [Reactive]
        public bool IsLeftStrip { get; set; } = true;

        [Reactive]
        public bool IsRightStrip { get; set; } = false;
    }
}
