using ConcurrentCollections;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Extensions;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using SpaceScience.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class SensorProvider : IProvider, IDynamic
{
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft = new();
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dictRight = new();
    private Dictionary<string, List<IFeature>> _cache = new();
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public SensorProvider()
    {

    }

    public string? CRS { get; set; }

    public IEnumerable<IFeature> Features => _featureCache;

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<Satellite>> observable)
    {
        observable.Subscribe(async s => await UpdateData(s));
    }

    private async Task UpdateData(IReadOnlyCollection<Satellite> satellites)
    {
        (_dictLeft, _dictRight) = await CreateDataAsync(satellites.ToList());

        _cache = satellites.ToDictionary(s => s.Name!, _ => new List<IFeature>());

        _featureCache.Clear();
    }

    // TODO: node/isShow refactoring
    public void ChangedData(Satellite satellite, int node, bool isShowLeft, bool isShowRight)
    {
        var name = satellite.Name;
        // TODO: node value refactoring
        //var node = satellite.CurrentNode - 1;
        //var isShowLeft = satellite.IsShow && satellite.IsLeftSwath;
        //var isShowRight = satellite.IsShow && satellite.IsRightSwath;

        if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
        {
            _cache[name].Clear();

            if (isShowLeft == true)
            {
                if (_dictLeft.ContainsKey(name) == true && _dictLeft[name].ContainsKey(node) == true)
                {
                    var features = _dictLeft[name][node];
                    _cache[name].AddRange(features);
                }
            }

            if (isShowRight == true)
            {
                if (_dictRight.ContainsKey(name) == true && _dictRight[name].ContainsKey(node) == true)
                {
                    var features = _dictRight[name][node];
                    _cache[name].AddRange(features);
                }
            }

            _featureCache.Clear();

            foreach (var item in _cache.SelectMany(s => s.Value))
            {
                _featureCache.Add(item);
            }

            DataHasChanged();
        }
    }

    private static async Task<(Dictionary<string, Dictionary<int, List<IFeature>>>, Dictionary<string, Dictionary<int, List<IFeature>>>)> CreateDataAsync(IList<Satellite> satellites)
    {
        return await Observable.Start(() =>
        {
            var dict = satellites.ToDictionary(
                s => s.Name!,
                s => s.ToOrbit().BuildSwaths(s.LookAngleDeg, s.RadarAngleDeg));

            var leftDict = dict.ToDictionary(
                s => s.Key,
                s => s.Value.ToFeature(s.Key, SpaceScience.Model.SwathDirection.Left));

            var rightDict = dict.ToDictionary(
                s => s.Key,
                s => s.Value.ToFeature(s.Key, SpaceScience.Model.SwathDirection.Right));

            return (leftDict, rightDict);
        }, RxApp.TaskpoolScheduler);
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
