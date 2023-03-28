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

internal class Sample4 : BaseSample
{
    public Sample4(Map map)
    {
        double a = 6948.0;
        double incl = 97.65;
        int fromNode = 1;
        int toNode = 2;
        double lookAngleDeg = 40.0;
        double radarAngleDeg = 16.0;
        double gam1Deg = lookAngleDeg - radarAngleDeg / 2.0; // 32.0; 
        double gam2Deg = lookAngleDeg + radarAngleDeg / 2.0; // 48.0; 

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);

        var tracks = orbit.BuildTracks();
        var swaths = orbit.BuildSwaths(lookAngleDeg, radarAngleDeg);

        var trackFeatures = tracks.ToFeature("");
        var leftFeatures = swaths.ToFeature("", SwathDirection.Left);
        var rightFeatures = swaths.ToFeature("", SwathDirection.Right);

        var targets = CreateTargets(3000);
        //var targets = CreateEquatorTargets(3000);
        //var targets = new List<(double,double,string)>() { (-30.01, -0.98, ""), (-27.4, -0.65, ""), (-20.75, 0.75, ""), (-18.25, 1.1, "") };
        //var targets = new List<(double,double,string)>() { (-18.23, 0.72, "") };
        //var targets = new List<(double,double,string)>() { (-18.28, 0.94, "") };

        var builder = new TimeWindowBuilder();
        var res = builder.BuildOnNodes(orbit, fromNode, toNode, gam1Deg, gam2Deg, targets);

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

        // track
        AddFeatures(trackFeatures[fromNode], layer1);
        AddFeatures(trackFeatures[toNode], layer1);
        // left
        AddFeatures(leftFeatures[fromNode], layer2);
        AddFeatures(leftFeatures[toNode], layer2);
        // right
        AddFeatures(rightFeatures[fromNode], layer2);
        AddFeatures(rightFeatures[toNode], layer2);

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
