using DynamicData;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Layers;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Layers;

public class FootprintProvider : IProvider, IDynamic, IFeatureProvider
{
    private readonly SourceList<Footprint> _footprints = new();
    private readonly ReadOnlyObservableCollection<IFeature> _features;

    public FootprintProvider()
    {
        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => FeatureBuilder.Build(s))
            .Bind(out _features)
            .Subscribe(_ => DataHasChanged());
    }

    public string? CRS { get; set; }

    public double MinVisible { get; set; } = 0;

    public double MaxVisible { get; set; } = double.MaxValue;

    public ReadOnlyObservableCollection<IFeature> Features => _features;

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<Footprint>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    public void UpdateData(IReadOnlyCollection<Footprint> footprints)
    {
        _footprints.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(footprints);
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
