using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;

namespace SpaceScienceSample.Samples;

public abstract class BaseSample
{
    protected static void AddFeatures(List<IFeature> features, WritableLayer layer)
    {
        layer.AddRange(features);
    }

    protected static IStyle CreatePointsStyle(Color color, double scale = 0.2)
    {
        return new SymbolStyle
        {
            SymbolType = SymbolType.Ellipse,
            Fill = new Brush(color),
            SymbolScale = scale,
        };
    }

    protected static IStyle CreateSwathStyle(Color color)
    {
        return new VectorStyle
        {
            Fill = new Brush(color),
            Opacity = 0.3f,
        };
    }

    protected static double LonConverter(double lonDeg)
    {
        while (lonDeg > 180) lonDeg -= 360.0;
        while (lonDeg < -180) lonDeg += 360.0;
        return lonDeg;
    }

    protected static List<(double lonDeg, double latDeg, string name)> CreateTargets(int counts)
    {
        var random = new Random();

        var targets = new List<(double, double, string)>();

        for (int i = 0; i < counts; i++)
        {
            var lat = (double)random.Next(-84, 84 + 1);
            var lon = (double)random.Next(-179, 179 + 1);

            targets.Add((lon, lat, $"{i + 1}"));
        }

        targets.Add((178, 70, $"{counts}"));
        targets.Add((177, 58, $"{counts + 1}"));

        return targets;
    }

    protected static List<(double lonDeg, double latDeg, string name)> CreateEquatorTargets(int counts)
    {
        var random = new Random();

        var targets = new List<(double, double, string)>();

        for (int i = 0; i < counts; i++)
        {
            var lat = (double)random.Next(-5, 5 + 1);
            var lon = (double)random.Next(-179, 179 + 1);

            targets.Add((lon, lat, $"{i + 1}"));
        }

        targets.Add((178, 70, $"{counts}"));
        targets.Add((177, 58, $"{counts + 1}"));

        return targets;
    }
}
