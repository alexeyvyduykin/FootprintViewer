using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Layers;

public class SatelliteProvider : MemoryProvider, IDynamic
{
    private static Random _random = new();
    private IProvider _provider = new MemoryProvider();
    private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _cache = new();
    private readonly Dictionary<string, List<IFeature>> _features = new();

    public SatelliteProvider()
    {

    }

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<Satellite>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    public void Update(string satelliteName, int node, bool show)
    {
        if (_features.ContainsKey(satelliteName) == true)
        {
            _features[satelliteName].Clear();

            if (show == true)
            {
                var keys = _cache[satelliteName].Keys;
                var min = keys.Min();
                var max = keys.Max();

                var min2 = Math.Min(node - 1, max);
                var node2 = Math.Max(min, min2);

                var res = _cache[satelliteName][node2];

                _features[satelliteName].AddRange(res);
            }

            _provider = new MemoryProvider(_features.SelectMany(s => s.Value));

            DataHasChanged();
        }
    }

    public void UpdateData(IReadOnlyCollection<Satellite> satellites)
    {
        _cache.Clear();
        _features.Clear();

        foreach (var item in satellites)
        {
            var d = item.BuildTracks();

            var a = FeatureBuilder.CreateTracks("", d);

            _cache.Add(item.Name, a);
            _features.Add(item.Name, new());
        }
    }

    public new string? CRS
    {
        get => _provider.CRS;
        set => _provider.CRS = value;
    }

    public new MRect? GetExtent()
    {
        return _provider.GetExtent();
    }

    public override async Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        return await _provider.GetFeaturesAsync(fetchInfo);
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
