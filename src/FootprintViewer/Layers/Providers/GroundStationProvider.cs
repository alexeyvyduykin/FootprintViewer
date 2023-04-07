using ConcurrentCollections;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using Splat;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class GroundStationProvider : IProvider, IDynamic
{
    private ConcurrentDictionary<string, List<IFeature>> _cache = new();
    private readonly IDataManager _dataManager;
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public GroundStationProvider(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.PlannedSchedules.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        Observable.StartAsync(UpdateImpl);
    }

    public string? CRS { get; set; }

    public IEnumerable<IFeature> Features => _featureCache;

    public ReactiveCommand<Unit, Unit> Update { get; }

    public event DataChangedEventHandler? DataChanged;

    private async Task UpdateImpl()
    {
        var ps = (await _dataManager.GetDataAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps != null)
        {
            _cache.Clear();

            foreach (var item in ps.GroundStations)
            {
                _cache.TryAdd(item.Name, new List<IFeature>());
            }

            _featureCache.Clear();
        }
    }

    // TODO: isShow refactoring
    public void ChangedData(GroundStation groundStation, double innerAngle, double[] arrAngles, bool isShow)
    {
        var name = groundStation.Name;
        var center = groundStation.Center;
        var angles = GetAngles(innerAngle, arrAngles);

        if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
        {
            _cache[name].Clear();

            if (isShow == true)
            {
                var gs = new GroundStation()
                {
                    Name = name,
                    Center = center,
                    Angles = angles,
                };

                _cache[name] = FeatureBuilder.Build(gs);
            }

            _featureCache.Clear();

            var features = _cache.SelectMany(s => s.Value).ToList();

            features.ForEach(s => _featureCache.Add(s));

            DataHasChanged();
        }

        static double[] GetAngles(double innerAngle, double[] angles)
        {
            var list = new List<double>
            {
                innerAngle
            };

            list.AddRange(angles);

            return list.ToArray();
        }
    }

    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        if (fetchInfo == null || fetchInfo.Extent == null)
        {
            throw new Exception();
        }

        var source = _featureCache;

        return Task.FromResult((IEnumerable<IFeature>)source);
    }

    public MRect? GetExtent() => GetExtent(Features);

    private static MRect? GetExtent(IEnumerable<IFeature> features)
    {
        MRect? mRect = null;

        foreach (IFeature feature in features)
        {
            if (feature.Extent != null)
            {
                mRect = ((mRect == null) ? feature.Extent : mRect.Join(feature.Extent));
            }
        }

        return mRect;
    }

    public void DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs(null, false, null));
    }
}
