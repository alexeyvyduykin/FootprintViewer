using FootprintViewer.Extensions;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;

namespace SpaceScienceSample.Samples;

internal class Sample1 : BaseSample
{
    public Sample1(Map map)
    {
        var layer = new WritableLayer();
        var pointsLayer1 = new WritableLayer() { Style = CreatePointsStyle(Color.Red) };

        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);
        var nodes = satellite.Nodes().Count;
        var track1 = new GroundTrack(orbit);

        track1.CalculateTrackWithLogStep(100);

        for (int i = 0; i < nodes; i++)
        {
            var list = track1.GetTrack(i);

            AddFeatures(new() { list.ToLineStringFeature(string.Empty) }, layer);
            AddFeatures(list.ToPointsFeatures(), pointsLayer1);
        }

        map.Layers.Add(layer);
        map.Layers.Add(pointsLayer1);
    }
}
