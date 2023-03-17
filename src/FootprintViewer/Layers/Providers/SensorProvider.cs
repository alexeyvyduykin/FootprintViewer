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

public class SensorProvider : IProvider, IDynamic
{
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dictLeft = new();
    private Dictionary<string, Dictionary<int, List<IFeature>>> _dictRight = new();
    private Dictionary<string, List<IFeature>> _cache = new();
    private readonly IDataManager _dataManager;
    private readonly ConcurrentHashSet<IFeature> _featureCache = new();

    public SensorProvider(IReadonlyDependencyResolver dependencyResolver)
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

        (_dictLeft, _dictRight) = await CreateDataAsync(satellites);

        _cache = satellites.ToDictionary(s => s.Name!, _ => new List<IFeature>());

        _featureCache.Clear();
    }

    public void ChangedData(SatelliteViewModel satellite)
    {
        var name = satellite.Name;
        var node = satellite.CurrentNode;
        var isShowLeft = satellite.IsShow && satellite.IsLeftSwath;
        var isShowRight = satellite.IsShow && satellite.IsRightSwath;

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
        return await Task.Run(() =>
        {
            var dictLeft = new Dictionary<string, Dictionary<int, List<IFeature>>>();
            var dictRight = new Dictionary<string, Dictionary<int, List<IFeature>>>();

            foreach (var sat in satellites)
            {
                var swaths = SpaceScienceBuilder.BuildSwaths(sat.ToPRDCTSatellite(), sat.LookAngleDeg, sat.RadarAngleDeg);

                var name = sat.Name!;
                var leftRes = FeatureBuilder.Build(name, swaths.Left);
                var rightRes = FeatureBuilder.Build(name, swaths.Right);

                dictLeft.Add(name, leftRes);
                dictRight.Add(name, rightRes);
            }

            return (dictLeft, dictRight);
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
