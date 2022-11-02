using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using NetTopologySuite.Geometries;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class SatelliteItemViewModel : ReactiveObject
    {
        public string? Name { get; set; }

        [Reactive]
        public bool IsActive { get; set; } = true;
    }

    public class FootprintTabFilter : BaseFilterViewModel<FootprintViewModel>
    {
        private readonly Data.DataManager.IDataManager _dataManager;

        public FootprintTabFilter(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();

            Satellites = new ObservableCollection<SatelliteItemViewModel>();

            IsLeftStrip = true;
            IsRightStrip = true;
            IsAllSatelliteActive = true;
            FromNode = 1;
            ToNode = 15;

            _dataManager.DataChanged
                .ToSignal()
                .InvokeCommand(ReactiveCommand.CreateFromTask(CreateSatelliteList));
        }

        public override IObservable<Func<FootprintViewModel, bool>> FilterObservable =>
            this.WhenAnyValue(s => s.FromNode, s => s.ToNode, s => s.IsLeftStrip, s => s.IsRightStrip, s => s.Switcher)
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(_ => this)
                .Select(CreatePredicate);

        private static Func<FootprintViewModel, bool> CreatePredicate(FootprintTabFilter filter)
        {
            return footprint =>
            {
                if (filter.Satellites.Where(s => s.IsActive == true).Select(s => s.Name).Contains(footprint.SatelliteName) == true)
                {
                    if (footprint.Node >= filter.FromNode && footprint.Node <= filter.ToNode)
                    {
                        if (footprint.Direction == SatelliteStripDirection.Left && filter.IsLeftStrip == true)
                        {
                            return true;
                        }

                        if (footprint.Direction == SatelliteStripDirection.Right && filter.IsRightStrip == true)
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
        }

        private async Task CreateSatelliteList()
        {
            var footprints = await _dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

            var satelliteNames = footprints.Select(s => s.SatelliteName).Distinct();

            if (satelliteNames != null)
            {
                var list = satelliteNames.OrderBy(s => s).Select(s => new SatelliteItemViewModel() { Name = s });

                Satellites = new ObservableCollection<SatelliteItemViewModel>(list);

                Satellites.ToObservableChangeSet()
                          .AutoRefresh(model => model.IsActive)
                          .Subscribe(s =>
                          {
                              Switcher = !Switcher;
                          });

                Switcher = !Switcher;
            }
        }

        public override bool Filtering(FootprintViewModel footprint)
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
        public ObservableCollection<SatelliteItemViewModel> Satellites { get; private set; }

        [Reactive]
        public Geometry? AOI { get; set; }

        public override string[]? Names => throw new NotImplementedException();
    }
}
