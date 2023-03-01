using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Fetcher;
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

public class GroundTargetProvider : IProvider, IDynamic, IFeatureProvider
{
    private readonly IDataManager _dataManager;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<IFeature> _features;

    public GroundTargetProvider(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        var styleManager = dependencyResolver.GetExistingService<LayerStyleManager>();

        MaxVisible = styleManager.MaxVisibleTargetStyle;

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => FeatureBuilder.Build(s))
            .Bind(out _features)
            .Subscribe(_ => DataHasChanged());

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

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

    public ReactiveCommand<Unit, Unit> Update { get; }

    public event DataChangedEventHandler? DataChanged;

    private async Task UpdateImpl()
    {
        var groundTargets = await _dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

        UpdateData(groundTargets);
    }

    public void UpdateData(IList<GroundTarget> groundTargets)
    {
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
            return Task.FromResult((IEnumerable<IFeature>)new List<IFeature>());
        }

        var source = Features.ToList();

        fetchInfo = new FetchInfo(fetchInfo);

        var source2 = source
            .Where(s => s != null && (s.Extent?.Intersects(fetchInfo.Extent) ?? false))
            .ToList();

        return Task.FromResult((IEnumerable<IFeature>)source2);
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

    public void DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs(null, false, null));
    }
}
