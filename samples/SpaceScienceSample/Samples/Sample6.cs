using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Extensions;
using FootprintViewer.Factories;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using SpaceScience;
using SpaceScience.Extensions;
using System.Linq;

namespace SpaceScienceSample.Samples;

internal class Sample6 : BaseSample
{
    public Sample6(Map map)
    {
        int node = 0;

        var gts = GroundTargetBuilder.Create(300);
        var satellites = SatelliteBuilder.Create(5);
        var gss = GroundStationBuilder.CreateDefault();

        var plannedSchedule = PlannedScheduleBuilder.Create(satellites, gts, gss);

        var factory = new SpaceScienceFactory();
        var sat = satellites.First();
        var orbit = sat.ToOrbit();

        double gam1Deg = sat.LookAngleDeg - sat.RadarAngleDeg / 2.0; // 32.0; 
        double gam2Deg = sat.LookAngleDeg + sat.RadarAngleDeg / 2.0; // 48.0; 

        var tracks = orbit.BuildTracks();
        var swaths = orbit.BuildSwaths(sat.LookAngleDeg, sat.RadarAngleDeg);

        var trackFeatures = tracks.ToFeature("");
        var leftFeatures = swaths.ToFeature("", SpaceScience.Model.SwathDirection.Left);
        var rightFeatures = swaths.ToFeature("", SpaceScience.Model.SwathDirection.Right);

        var footprints = plannedSchedule.PlannedSchedules
            .Where(s => s is ObservationTaskResult)
            .Cast<ObservationTaskResult>()
            .Select(s => new Footprint()
            {
                Center = s.Geometry.Center,
                Border = s.Geometry.Border
            }).ToList();

        var footprintFeatures =
            footprints
            //   .Where(s => s.Node == node + 1)
            .Select(s => FeatureBuilder.Build(s))
            .ToList();

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
