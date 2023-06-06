﻿using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
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
    private static Random _random = new();
    private IProvider _provider = new MemoryProvider();

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
            .Select(FeatureBuilder.Build));

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
}