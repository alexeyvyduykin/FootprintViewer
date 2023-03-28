using FootprintViewer.Extensions;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using System.Linq;

namespace SpaceScienceSample.Samples;

internal class Sample3 : BaseSample
{
    public Sample3(Map map)
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
        var satellite = factory.CreateSatellite(orbit);

        var tracks = satellite.BuildTracks();
        var swaths = satellite.BuildSwaths(lookAngleDeg, radarAngleDeg);

        var trackFeatures = tracks.ToFeature("");
        var leftFeatures = swaths.ToFeature("", SwathDirection.Left);
        var rightFeatures = swaths.ToFeature("", SwathDirection.Right);

        var targets = CreateTargets(300);

        var builder = new TimeWindowBuilder();
        var res = builder.BuildOnNode(satellite, node, gam1Deg, gam2Deg, targets);

        var features = targets.Select(s => (s.Item1, s.Item2)).ToList().ToPointsFeatures();

        var availableFeatures = res.Select(s => (s.Lon, s.Lat)).ToList().ToPointsFeatures();

        var intervalFeatures = res.SelectMany(s => s.Interval.Select(t => t.ToLineStringFeature(""))).ToList();
        var directionFeatures = res.SelectMany(s => s.Direction.Select(t => t.ToLineStringFeature(""))).ToList();

        var gtLayer = new WritableLayer() { Style = CreatePointsStyle(Color.Black) };
        var layer1 = new WritableLayer();
        var layer2 = new WritableLayer() { Style = CreateSwathStyle(Color.Blue) };
        var layer3 = new WritableLayer() { Style = CreatePointsStyle(Color.Green) };
        var layer5 = new WritableLayer() { Style = CreateIntervalStyle(Color.Green) };

        AddFeatures(features, gtLayer);

        AddFeatures(trackFeatures[node], layer1);
        AddFeatures(leftFeatures[node], layer2);
        AddFeatures(rightFeatures[node], layer2);
        AddFeatures(availableFeatures, layer3);
        AddFeatures(intervalFeatures, layer5);
        AddFeatures(directionFeatures, layer5);

        map.Layers.Add(gtLayer);
        map.Layers.Add(layer1);
        map.Layers.Add(layer2);
        map.Layers.Add(layer3);
        map.Layers.Add(layer5);
    }

    private static IStyle CreateIntervalStyle(Color color)
    {
        return new VectorStyle
        {
            Line = new Pen(color, 5.0),
        };
    }
}
