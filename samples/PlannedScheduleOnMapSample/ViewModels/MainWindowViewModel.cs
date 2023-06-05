﻿using FootprintViewer;
using FootprintViewer.Data.Models;
using FootprintViewer.Factories;
using FootprintViewer.Helpers;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Interactivity.UI;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using PlannedScheduleOnMapSample.Layers;
using ReactiveUI.Fody.Helpers;
using System;

namespace PlannedScheduleOnMapSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private const int _maxVisibleFootprintStyle = 10000;
    private ISelector? _selector;
    private readonly FeatureManager _featureManager;
    private const string SelectField = InteractiveFields.Select;
    private const string HoverField = InteractiveFields.Hover;

    public static MainWindowViewModel Instance = new();

    public MainWindowViewModel()
    {
        Map = new Map();
        Map.AddLayer(new Layer(), LayerType.WorldMap);

        var provider = new FootprintProvider();
        Map.Layers.Add(CreateLayer(provider));

        string path = System.IO.Path.Combine(EnvironmentHelpers.GetFullBaseDirectory(), "Assets", "world.mbtiles");
        var resource = new MapResource("world", path);
        Map.SetWorldMapLayer(resource);

        PlannedScheduleTab = new();
        PlannedScheduleTab.ToLayerProvider(provider);

        _featureManager = new FeatureManager()
            .WithSelect(f => f[SelectField] = true)
            .WithUnselect(f => f[SelectField] = false)
            .WithEnter(f => f[HoverField] = true)
            .WithLeave(f => f[HoverField] = false);

        SelectCommand();
    }

    private ILayer CreateLayer(IProvider provider)
    {
        var style = CreateFootprintLayerStyle();

        var layer = new Layer() { Name = "FootrpintLayer", DataSource = provider, Style = style, IsMapInfoLayer = true };

        return layer;
    }

    private void SelectCommand()
    {
        _selector = new InteractiveBuilder()
            .SelectSelector<Selector>()
            .AttachTo(Map)
            .Build();

        _selector.Select.Subscribe(s =>
        {
            SelectFeature(s.Feature, s.Layer);
        });

        _selector.Unselect.Subscribe(s =>
        {
            UnselectFeature(s.Layer);
        });

        //_selector.HoverBegin.Subscribe(s =>
        //{
        //    EnterFeature(s.Feature, s.Layer);
        //});

        //_selector.HoverEnd.Subscribe(s =>
        //{
        //    LeaveFeature(s.Layer);
        //});

        Interactive = _selector;
        State = States.Selecting;
    }

    private void EnterFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Enter(feature);
    }

    private void LeaveFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Leave();
    }

    private void SelectFeature(IFeature feature, ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Select(feature);
    }

    private void UnselectFeature(ILayer layer)
    {
        _featureManager
            .OnLayer(layer)
            .Unselect();
    }

    private static IStyle CreateFootprintLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            if (gf.Geometry is Point)
            {
                return null;
            }

            if (gf[SelectField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Green, 0.55f)),
                    Outline = new Pen(Color.Black, 4.0),
                    Line = new Pen(Color.Black, 4.0)
                };
            }

            if (gf[HoverField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Green, 0.85f)),
                    Outline = new Pen(Color.Yellow, 3.0),
                    Line = new Pen(Color.Yellow, 3.0)
                };
            }

            return new VectorStyle()
            {
                Fill = new Brush(Color.Opacity(Color.Green, 0.25f)),
                Line = new Pen(Color.Green, 1.0),
                Outline = new Pen(Color.Green, 1.0),
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
            };
        });
    }

    public PlannedScheduleTabViewModel PlannedScheduleTab { get; set; }

    public Map Map { get; private set; }

    [Reactive]
    public IInteractive? Interactive { get; set; }

    [Reactive]
    public string State { get; set; } = States.Default;
}
