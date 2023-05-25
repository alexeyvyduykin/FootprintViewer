using ConcurrentCollections;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class GroundStationProvider : IProvider, IDynamic
{
    private readonly ConcurrentDictionary<string, List<IFeature>> _cache = new();
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public GroundStationProvider()
    {

    }

    public string? CRS { get; set; }

    public IEnumerable<IFeature> Features => _featureCache;

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<GroundStation>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    private void UpdateData(IReadOnlyCollection<GroundStation> groundStations)
    {
        _cache.Clear();

        foreach (var item in groundStations)
        {
            _cache.TryAdd(item.Name, new List<IFeature>());
        }

        _featureCache.Clear();
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
