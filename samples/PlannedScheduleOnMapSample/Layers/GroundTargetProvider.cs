using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Layers;

public class GroundTargetProvider : MemoryProvider, IDynamic
{
    private IProvider _provider = new MemoryProvider();

    public GroundTargetProvider()
    {

    }

    public event DataChangedEventHandler? DataChanged;

    public void SetObservable(IObservable<IReadOnlyCollection<GroundTarget>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    public void UpdateData(IReadOnlyCollection<GroundTarget> groundTargets)
    {
        var features = new List<IFeature>(groundTargets
            .Select(s =>
            {
                var feature = FeatureBuilder.CreateGroundTarget(s);

                //feature["Satellite"] = s.SatelliteName;
                //feature["Node"] = s.Node;
                //feature["Direction"] = s.Direction.ToString();
                //feature["Target"] = s.TargetName;

                return feature;
            }));

        _provider = new MemoryProvider(features);

        DataHasChanged();
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
