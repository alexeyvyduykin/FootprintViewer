using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using FootprintViewer.Extensions;
using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Styles;
using SpaceScience;
using SpaceScience.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Samples;

internal class Sample5 : BaseSample
{
    public Sample5(Map map)
    {
        double a = 6948.0;
        double incl = 97.65;
        int node = 1;
        double lookAngleDeg = 40.0;
        double radarAngleDeg = 16.0;
        double gam1Deg = lookAngleDeg - radarAngleDeg / 2.0; // 32.0; 
        double gam2Deg = lookAngleDeg + radarAngleDeg / 2.0; // 48.0; 

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);

        var tracks = orbit.BuildTracks();
        var swaths = orbit.BuildSwaths(lookAngleDeg, radarAngleDeg);

        var trackFeatures = tracks.ToFeature("");
        var leftFeatures = swaths.ToFeature("", SpaceScience.Model.SwathDirection.Left);
        var rightFeatures = swaths.ToFeature("", SpaceScience.Model.SwathDirection.Right);

        var satellite2 = new Satellite()
        {
            Name = "Satellite2",
            Epoch = DateTime.Now,
            Semiaxis = a,
            Eccentricity = 0,
            InclinationDeg = incl,
            ArgumentOfPerigeeDeg = 0,
            LongitudeAscendingNodeDeg = 0,
            RightAscensionAscendingNodeDeg = 0,
            Period = orbit.Period,
            LookAngleDeg = lookAngleDeg,
            RadarAngleDeg = radarAngleDeg
        };

        var footprints = FootprintBuilder.Create(new List<Satellite>() { satellite2 }, 1000);

        var footprintFeatures =
            footprints
            .Where(s => s.Node == node + 1)
            .Select(s => FeatureBuilder.Build(s))
            .ToList();

        //var footprintFeatures =
        //    footprints
        //    .Select(s => s.Points!.Coordinates.Select(s => (s.X, s.Y)).ToList())
        //    .Select(s => s.ToPolygonFeature(""))
        //    .ToList();

        var layer1 = new WritableLayer();
        var layer2 = new WritableLayer() { Style = CreateSwathStyle(Color.Blue) };
        var layer3 = new WritableLayer() { Style = CreateFootprintStyle(Color.Green) };

        // track
        AddFeatures(trackFeatures[node], layer1);
        // left
        AddFeatures(leftFeatures[node], layer2);
        // right
        AddFeatures(rightFeatures[node], layer2);

        AddFeatures(footprintFeatures, layer3);

        map.Layers.Add(layer1);
        map.Layers.Add(layer2);
        map.Layers.Add(layer3);
    }

    private static IStyle CreateFootprintStyle(Color color)
    {
        return new VectorStyle
        {
            Outline = new Pen(color, 3.0),
            Fill = new Brush(color),
            Opacity = 0.3f,
        };
    }
}
