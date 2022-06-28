using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SatelliteItem : ReactiveObject
    {
        public string? Name { get; set; }

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class FootprintObserverFilter : ViewerListFilter<FootprintInfo>
    {
        private readonly IProvider<Satellite> _satelliteProvider;

        public FootprintObserverFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            _satelliteProvider = dependencyResolver.GetExistingService<IProvider<Satellite>>();

            Satellites = new ObservableCollection<SatelliteItem>();

            IsLeftStrip = true;
            IsRightStrip = true;
            IsAllSatelliteActive = true;
            FromNode = 1;
            ToNode = 15;

            this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftStrip, s => s.IsRightStrip, s => s.Switcher)
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(_ => Unit.Default)
                .InvokeCommand(Update);

            Observable.StartAsync(CreateSatelliteList).Subscribe();
        }

        private async Task CreateSatelliteList()
        {
            var satellites = await _satelliteProvider.GetNativeValuesAsync(null);
            var satelliteNames = satellites?.Select(s => s.Name).ToList();

            if (satelliteNames != null)
            {
                var list = satelliteNames.OrderBy(s => s).Select(s => new SatelliteItem() { Name = s });

                Satellites = new ObservableCollection<SatelliteItem>(list);

                Satellites.ToObservableChangeSet()
                          .AutoRefresh(model => model.IsActive)
                          .Subscribe(s =>
                          {
                              Switcher = !Switcher;
                          });

                Switcher = !Switcher;
            }
        }

        public override bool Filtering(FootprintInfo footprint)
        {
            if (Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
            {
                if (footprint.Node >= FromNode && footprint.Node <= ToNode)
                {
                    if (footprint.Direction == SatelliteStripDirection.Left && IsLeftStrip == true)
                    {
                        return true;
                    }

                    if (footprint.Direction == SatelliteStripDirection.Right && IsRightStrip == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [Reactive]
        private bool Switcher { get; set; }

        [Reactive]
        public int FromNode { get; set; }

        [Reactive]
        public int ToNode { get; set; }

        [Reactive]
        public bool IsLeftStrip { get; set; }

        [Reactive]
        public bool IsRightStrip { get; set; }

        [Reactive]
        public bool IsAllSatelliteActive { get; set; }

        [Reactive]
        public ObservableCollection<SatelliteItem> Satellites { get; private set; }

        [Reactive]
        public Geometry? AOI { get; set; }

        public override string[]? Names => throw new NotImplementedException();
    }
}
