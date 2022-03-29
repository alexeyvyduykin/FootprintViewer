using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public class SatelliteInfo : ReactiveObject
    {
        private readonly string _name;
        private readonly Satellite _satellite;
        private readonly int _minNode = 1;
        private readonly int _maxNode;

        public SatelliteInfo(Satellite satellite)
        {
            _satellite = satellite;

            _name = satellite.Name!;

            var count = satellite.ToPRDCTSatellite().Nodes().Count;
       
            _maxNode = count;

            CurrentNode = 1;

            ShowInfoClick = ReactiveCommand.Create(ShowInfoClickImpl);
        }

        private void ShowInfoClickImpl() => IsShowInfo = !IsShowInfo;

        public ReactiveCommand<Unit, Unit> ShowInfoClick { get; }

        public string? Name => _name;

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
