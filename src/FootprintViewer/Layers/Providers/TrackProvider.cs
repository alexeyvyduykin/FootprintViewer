using ConcurrentCollections;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using SpaceScience;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class TrackProvider : IProvider, IDynamic
{
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dict = new();
    private Dictionary<string, List<IFeature>> _cache = new();
    private readonly IDataManager _dataManager;
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public TrackProvider(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Satellites.ToString()))
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
        var satellites = await _dataManager.GetDataAsync<Satellite>(DbKeys.Satellites.ToString());

        _dict = await CreateDataAsync(satellites);

        _cache = satellites.ToDictionary(s => s.Name!, _ => new List<IFeature>());

        _featureCache.Clear();
    }

    public void ChangedData(SatelliteViewModel satellite)
    {
        var name = satellite.Name;
        var node = satellite.CurrentNode;
        var isShow = satellite.IsShow && satellite.IsTrack;

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
        return await Task.Run(() =>
        {
            var tracks = satellites.Select(s => (s.Name!, SpaceScienceBuilder.BuildTracks(s.ToPRDCTSatellite())));

            var _dict = new Dictionary<string, Dictionary<int, List<IFeature>>>();

            foreach (var (satName, track) in tracks)
            {
                var dict = FeatureBuilder.BuildTrack(satName, track.Track);

                _dict.Add(satName, dict);
            }

            return _dict;
        });
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
