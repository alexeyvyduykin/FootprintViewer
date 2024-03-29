﻿using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Utilities;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class TrackProvider : IProvider, IDynamic
{
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dict = new();
    private Dictionary<string, List<IFeature>> _cache = new();
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public TrackProvider()
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
        _dict = await CreateDataAsync(satellites.ToList());

        _cache = satellites.ToDictionary(s => s.Name!, _ => new List<IFeature>());

        _featureCache.Clear();
    }

    // TODO: node/isShow refactoring
    public void ChangedData(Satellite satellite, int node, bool isShow)
    {
        var name = satellite.Name;
        // TODO: node value refactoring
        //var node = satellite.CurrentNode - 1;
        //var isShow = satellite.IsShow && satellite.IsTrack;

        if (string.IsNullOrEmpty(name) == false && _cache.ContainsKey(name) == true)
        {
            _cache[name].Clear();

            if (isShow == true)
            {
                if (_dict.ContainsKey(name) == true && _dict[name].ContainsKey(node) == true)
                {
                    var features = _dict[name][node];
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

    private static async Task<Dictionary<string, Dictionary<int, List<IFeature>>>> CreateDataAsync(IList<Satellite> satellites)
    {
        return await Observable.Start(() =>
        {
            return satellites.ToDictionary(
                s => s.Name!,
                s => FeatureBuilder.CreateTracks(s.Name!, s.BuildTracks()));
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
