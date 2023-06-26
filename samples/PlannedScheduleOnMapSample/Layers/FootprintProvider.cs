﻿using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlannedScheduleOnMapSample.Layers;

public class FootprintProvider : MemoryProvider
{
    private MemoryProvider _provider = new MemoryProvider();

    public FootprintProvider()
    {

    }

    public void SetObservable(IObservable<IReadOnlyCollection<Footprint>> observable)
    {
        observable.Subscribe(UpdateData);
    }

    public void UpdateData(IReadOnlyCollection<Footprint> footprints)
    {
        var features = new List<IFeature>(footprints
            .Select(s =>
            {
                var feature = FeatureBuilder.CreateFootprint(s);

                feature["Satellite"] = s.SatelliteName;
                feature["Node"] = s.Node;
                feature["Direction"] = s.Direction.ToString();
                feature["Target"] = s.TargetName;

                return feature;
            }));

        _provider = new MemoryProvider(features);
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

    public new IFeature? Find(object? value, string fieldName)
    {
        var fdfdf = _provider.Features.First()["Name"];
        // var res = _provider.Find(value, fieldName);


        foreach (var item in _provider.Features)
        {
            if (string.Equals(item["Name"], value))
            {
                return item;
            }
        }

        return null;
    }
}