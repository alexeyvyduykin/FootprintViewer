using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Layers.Providers;

public class GroundTargetProvider : IProvider
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<IFeature> _features;
    private readonly SourceList<IFeature> _activeFeatures = new();
    private readonly ReadOnlyObservableCollection<IFeature> _activeFeaturesItems;
    private MRect? _lastExtent;

    public GroundTargetProvider(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => FeatureBuilder.Build(s))
            .Bind(out _features)
            .Subscribe();

        _activeFeatures
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _activeFeaturesItems)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);
        ActiveFeaturesChanged = ReactiveCommand.Create<IEnumerable<IFeature>?, string[]?>(ActiveFeaturesChangedImpl);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.GroundTargets.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        Observable.StartAsync(UpdateImpl);
    }

    public string? CRS { get; set; }

    public double MinVisible { get; set; } = 0;

    public double MaxVisible { get; set; } = double.MaxValue;

    public ReadOnlyObservableCollection<IFeature> Features => _features;

    public ReadOnlyObservableCollection<IFeature> ActiveFeatures => _activeFeaturesItems;

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<IEnumerable<IFeature>?, string[]?> ActiveFeaturesChanged { get; }

    private async Task UpdateImpl()
    {
        var groundTargets = await _dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

        _groundTargets.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(groundTargets);
        });
    }

    public virtual Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        if (fetchInfo == null || fetchInfo.Extent == null)
        {
            throw new Exception();
        }

        if (MinVisible > fetchInfo.Resolution || MaxVisible < fetchInfo.Resolution)
        {
            ActiveFeaturesChanged.Execute(null).Subscribe();

            return Task.FromResult((IEnumerable<IFeature>)new List<IFeature>());
        }

        var source = Features.ToList();

        fetchInfo = new FetchInfo(fetchInfo);

        var source2 = source
            .Where(s => s != null && (s.Extent?.Intersects(fetchInfo.Extent) ?? false))
            .ToList();

        if (fetchInfo.Extent.Equals(_lastExtent) == false)
        {
            ActiveFeaturesChanged.Execute(source2).Subscribe();

            _lastExtent = fetchInfo.Extent;
        }

        return Task.FromResult((IEnumerable<IFeature>)source2);
    }

    private string[]? ActiveFeaturesChangedImpl(IEnumerable<IFeature>? activeFeatures)
    {
        if (activeFeatures == null)
        {
            _activeFeatures.Edit(innerList =>
            {
                innerList.Clear();
            });

            return null;
        }

        _activeFeatures.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(activeFeatures);
        });

        return activeFeatures
            .Where(s => s.Fields.Contains("Name"))
            .Select(s => (string)s["Name"]!)
            .ToArray();
    }

    public IFeature? Find(object? value, string fieldName)
    {
        object? value2 = value;
        string fieldName2 = fieldName;
        return Features.FirstOrDefault((IFeature f) => value2 != null && f[fieldName2] == value2);
    }

    public MRect? GetExtent() => GetExtent(Features);

    private static MRect? GetExtent(IReadOnlyList<IFeature> features)
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
}
