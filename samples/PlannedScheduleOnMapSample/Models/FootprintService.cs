using FootprintViewer;
using FootprintViewer.Data;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using PlannedScheduleOnMapSample.Layers;
using PlannedScheduleOnMapSample.ViewModels;
using SpaceScience.Extensions;
using SpaceScience.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using static SpaceScience.Extensions.OrbitExtensions;

namespace PlannedScheduleOnMapSample.Models;

public class FootprintService
{
    private readonly Map _map;
    private readonly QueueLayer _trackLayer;
    private readonly DataManager _dataManager;

    public FootprintService(Map map, DataManager dataManager)
    {
        _map = map;
        _dataManager = dataManager;

        _trackLayer = (QueueLayer)_map.Layers.FindLayer(MainWindowViewModel.TrackKey).FirstOrDefault()!;
    }

    public async Task ShowTrackAsync(IFeature feature)
    {
        var footprintName = (string)feature["Name"]!;
        var satelliteName = (string)feature["Satellite"]!;
        var node = (int)feature["Node"]!;
        var targetName = (string)feature["Target"]!;

        var res = (await _dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey)).FirstOrDefault()!;

        var observationTask = (ObservationTaskResult)res.PlannedSchedules.Where(s => Equals($"Footprint_{s.TaskName}", footprintName)).FirstOrDefault()!;

        var begin = observationTask.Interval.Begin;
        var duration = observationTask.Interval.Duration;

        var direction = observationTask.Direction.ToString();

        var groundTarget = res.GroundTargets.Where(s => Equals(s.Name, targetName)).FirstOrDefault()!;

        var satellite = res.Satellites.Where(s => Equals(s.Name, satelliteName)).FirstOrDefault()!;
        var orbit = satellite.ToOrbit();
        var epoch = satellite.Epoch;
        var period = satellite.Period;
        var radarAngle = satellite.RadarAngleDeg;
        var lookAngle = satellite.LookAngleDeg;

        var features = FeatureBuilder.CreateTracks("", satellite.BuildTracks());

        var track = new GroundTrack(orbit);
        var begin0 = begin.AddSeconds(-period * (node - 1));

        var t0 = (begin0 - epoch).TotalSeconds;
        var t1 = t0 + duration;

        track.CalculateTrackOnTimeInterval(t0, t1, 2);

        var trackLine = track.GetTrack(node - 1, duration, LonConverters.Default);//.ToCutList();

        track.CalculateTrackOnTimeInterval(t0 - 60, t1 + 60, 10);

        var trackLineBase = track.GetTrack(node - 1, duration + 2 * 60, LonConverters.Default);//.ToCutList();

        var trackFeature = FeatureBuilder.CreateTrack("FootprintTrack", trackLine);
        var trackFeatureBase = FeatureBuilder.CreateTrack("BaseTrack", trackLineBase);

        var (baseNear, baseFar) = orbit.BuildSwaths(node - 1, t0 - 60, t1 + 60, 10, lookAngle, radarAngle, Enum.Parse<SpaceScience.Model.SwathDirection>(direction));
        var baseNearSwathFeature = FeatureBuilder.CreateTrack("BaseSwath", baseNear);
        var baseFarSwathFeature = FeatureBuilder.CreateTrack("BaseSwath", baseFar);

        var (near, far) = orbit.BuildSwaths(node - 1, t0, t1, 2, lookAngle, radarAngle, Enum.Parse<SpaceScience.Model.SwathDirection>(direction));
        var nearSwathFeature = FeatureBuilder.CreateTrack("FootprintSwath", near);
        var farSwathFeature = FeatureBuilder.CreateTrack("FootprintSwath", far);

        var dd = trackLineBase.TakeLast(2).ToList();
        var (x1, y1) = dd[0];
        var (x2, y2) = dd[1];
        var arrow = CreateArrow(x1, y1, x2, y2);

        var arrowFeature = arrow.ToFeatureEx("Arrow");

        var fTarget = FeatureBuilder.CreateGroundTarget(groundTarget);

        var firstPoint = trackLine.First();
        var lastPoint = trackLine.Last();
        var firstNearPoint = near.First();
        var lastNearPoint = near.Last();
        var firstFarPoint = far.First();
        var lastFarPoint = far.Last();

        var f1 = FeatureBuilder.CreateLineString("", new() { firstPoint, firstNearPoint, firstFarPoint });
        var f2 = FeatureBuilder.CreateLineString("", new() { lastPoint, lastNearPoint, lastFarPoint });

        var p1 = FeatureBuilder.CreatePolygon(new() { firstNearPoint, lastNearPoint, lastFarPoint, firstFarPoint }, "AreaPoly");

        _trackLayer.Clear();
        
        _trackLayer.Add(p1);


        _trackLayer.Add(trackFeatureBase);
        _trackLayer.Add(trackFeature);
        _trackLayer.Add(baseNearSwathFeature);
        _trackLayer.Add(baseFarSwathFeature);
      //  _trackLayer.Add(nearSwathFeature);
      //  _trackLayer.Add(farSwathFeature);
        _trackLayer.Add(arrowFeature);
        _trackLayer.Add(fTarget);
       // _trackLayer.Add(f1);
       // _trackLayer.Add(f2);
    
        _trackLayer.DataHasChanged();
    }

    private Polygon CreateArrow(double x11, double y11, double x22, double y22)
    {
        var (x1, y1) = SphericalMercator.FromLonLat(x11, y11);
        var (x2, y2) = SphericalMercator.FromLonLat(x22, y22);

        // Backward direction vector
        var (dx, dy) = (x1 - x2, y1 - y2);

        // Length of it:
        var norm = Math.Sqrt(dx * dx + dy * dy);

        // Normalize it: uD =

        var (udx, udy) = (dx / norm, dy / norm);

        // To form "wings" of arrow, rotate uD by needed angle. For example, I use angle Pi / 6 with Cos(Pi/ 6) = Sqrt(3) / 2 and Sin(Pi/ 6) = 1 / 2

        var angle = Math.PI / 3;// Math.PI / 6;
        var cosa = Math.Cos(angle);
        var sina = Math.Sin(angle);

        var ax = udx * cosa - udy * sina;
        var ay = udx * sina + udy * cosa;
        var bx = udx * cosa + udy * sina;
        var by = -udx * sina + udy * cosa;

        // Points for head with wing length L = 20:
        double len = 50000;// 0.5;
        var (a, b) = (x1 + len * ax, y1 + len * ay);
        var (c, d) = (x1 + len * bx, y1 + len * by);

        var list = new (double x, double y)[]
        {
            (x2, y2),
            (a, b),
            (c, d),
            (x2, y2),
        };

        var coordinates = list
           // .Select(s => SphericalMercator.FromLonLat(s.Item1, s.Item2))
            .Select(s => new Coordinate(s.x, s.y))
            .ToArray();

        var gf = new GeometryFactory();
        var poly = gf.CreatePolygon(coordinates);

        return poly!;

    }
}
