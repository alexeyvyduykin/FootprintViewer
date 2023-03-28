using FootprintViewer.Extensions;
using Mapsui;
using Mapsui.Interactivity;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;

namespace SpaceScienceSample.Samples;

internal class Sample2 : BaseSample
{
    public Sample2(Map map)
    {
        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl, 50.0);

        var tracks = orbit.BuildTracks();
        var swaths = orbit.BuildSwaths(40, 16);

        var features = tracks.ToFeature("Satellite1");
        var leftFeatures = swaths.ToFeature("SatelliteLeft1", SwathDirection.Left);
        var rightFeatures = swaths.ToFeature("SatelliteRight1", SwathDirection.Right);

        var trackVertices = tracks.ToFeatureVertices();
        var leftVertices = swaths.ToFeatureVertices(SwathDirection.Left);
        var rightVertices = swaths.ToFeatureVertices(SwathDirection.Right);

        var layer1 = new WritableLayer();
        var layer2 = new WritableLayer() { Style = CreateSwathStyle(Color.Green) };
        var pointsLayer1 = new WritableLayer() { Style = CreatePointsStyle(Color.Red) };
        var pointsLayer2 = new WritableLayer() { Style = CreatePointsStyle(Color.Blue) };

        AddFeatures(features[0], layer1);
        AddFeatures(features[1], layer1);

        AddFeatures(leftFeatures[0], layer2);
        AddFeatures(rightFeatures[0], layer2);
        AddFeatures(leftFeatures[1], layer2);
        AddFeatures(rightFeatures[1], layer2);

        AddFeatures(trackVertices[0], pointsLayer1);
        AddFeatures(trackVertices[1], pointsLayer1);

        AddFeatures(leftVertices[0], pointsLayer2);
        AddFeatures(rightVertices[0], pointsLayer2);
        AddFeatures(leftVertices[1], pointsLayer2);
        AddFeatures(rightVertices[1], pointsLayer2);

        map.Layers.Add(layer1);
        map.Layers.Add(layer2);
        map.Layers.Add(pointsLayer1);
        map.Layers.Add(pointsLayer2);
    }
}
