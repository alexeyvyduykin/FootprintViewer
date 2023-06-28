using FootprintViewer;
using FootprintViewer.Data;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Geometries;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;
using NetTopologySuite.Geometries;
using PlannedScheduleOnMapSample.Models;
using PlannedScheduleOnMapSample.ViewModels;
using ReactiveUI;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using static SpaceScience.Extensions.OrbitExtensions;

namespace PlannedScheduleOnMapSample.Layers;

public class FootprintPreviewProvider : MemoryProvider, IDynamic
{
    private IProvider _provider = new MemoryProvider();
    private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _cache = new();
    private readonly Dictionary<string, Dictionary<int, List<IFeature>>> _ivalCache = new();
    private List<Satellite> _satellites = new();
    private List<GroundTarget> _gts = new();

    public FootprintPreviewProvider(IDataManager dataManager)
    {
        Observable.Start(async () =>
        {
            var res = (await dataManager.GetDataAsync<PlannedScheduleResult>(MainWindowViewModel.PlannedScheduleKey)).FirstOrDefault()!;

            _satellites = res.Satellites.ToList();
            _gts = res.GroundTargets.ToList();
        }, RxApp.TaskpoolScheduler);
    }

    public event DataChangedEventHandler? DataChanged;

    public void Update(ObservationTaskResult observationTask, bool isPreview, bool isFullTrack, bool isSwath, bool isGt)
    {
        var list = new List<IFeature>();

        if (isPreview == true)
        {
            var satelliteName = observationTask.SatelliteName;
            var node = observationTask.Node + 1;
            var begin = observationTask.Interval.Begin;
            var duration = observationTask.Interval.Duration;
            var direction = observationTask.Direction.ToString();
            var satellite = _satellites.Where(s => Equals(s.Name, satelliteName)).FirstOrDefault()!;
            var epoch = satellite.Epoch;
            var period = satellite.Period;
            var begin0 = begin.AddSeconds(-period * (node - 1));
            var t0 = (begin0 - epoch).TotalSeconds;
            var t1 = t0 + duration;

            if (isFullTrack == false)
            {
                list.AddRange(GetSegmentTrackFeatures(satellite, node, t0, t1, duration));
            }
            else
            {
                list.AddRange(GetFullTrackFeatures(satelliteName, node));
            }

            if (isSwath == true)
            {
                list.AddRange(GetSwathFeatures(observationTask, satellite, t0, t1));
            }

            if (isGt == true)
            {
                list.Add(GetGroundTargetFeature(observationTask));
            }
        }

        _provider = new MemoryProvider(list);

        DataHasChanged();
    }

    public void SetObservable(IObservable<PlannedScheduleResult> observable)
    {
        observable.Subscribe(UpdateData);
    }

    private void UpdateData(PlannedScheduleResult plannedSchedule)
    {
        _cache.Clear();
        _ivalCache.Clear();

        var res = plannedSchedule.BuildObservableIntervals();

        foreach (var item in plannedSchedule.Satellites)
        {
            var d = item.BuildTracks();

            var a = FeatureBuilder.CreateTracks("BaseTrack", d);

            var b = FeatureBuilder.CreateTracks("FootprintTrack", res[item.Name]);

            _cache.Add(item.Name, a);
            _ivalCache.Add(item.Name, b);
        }
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

    public void DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs(null, false, null));
    }

    private List<IFeature> GetSegmentTrackFeatures(Satellite satellite, int node, double t0, double t1, double duration)
    {
        var orbit = satellite.ToOrbit();

        var track = new GroundTrack(orbit);

        track.CalculateTrackOnTimeInterval(t0, t1, 2);
        var res1 = track.GetTrack(node - 1, duration);
        var trackLines = LonSplitters.Default.Split(res1);

        track.CalculateTrackOnTimeInterval(t0 - 60, t1 + 60, 10);
        var res2 = track.GetTrack(node - 1, duration + 2 * 60);
        var baseTrackLines = LonSplitters.Default.Split(res2);

        var trackFeatures = trackLines.Select(s => FeatureBuilder.CreateTrack("FootprintTrack", s)).ToList();
        var baseTrackFeatures = baseTrackLines.Select(s => FeatureBuilder.CreateTrack("BaseTrack", s)).ToList();

        var dd = baseTrackLines.Last().TakeLast(2).ToList();
        var (x1, y1) = dd[0];
        var (x2, y2) = dd[1];
        var arrow = CreateArrow(x1, y1, x2, y2);

        var arrowFeature = arrow.ToFeatureEx(style: StyleBuilder.CreateArrowStyle());

        var features = new List<IFeature>();
        features.AddRange(baseTrackFeatures);
        features.AddRange(trackFeatures);
        features.Add(arrowFeature);

        return features;
    }

    private List<IFeature> GetFullTrackFeatures(string satelliteName, int node)
    {
        var list = new List<IFeature>();

        var keys = _cache[satelliteName].Keys;
        var min = keys.Min();
        var max = keys.Max();

        var min2 = Math.Min(node - 1, max);
        var node2 = Math.Max(min, min2);

        var res1 = _cache[satelliteName][node2];
        var res2 = _ivalCache[satelliteName][node2];

        list.AddRange(res1);
        list.AddRange(res2);

        return list;
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

    private List<IFeature> GetSwathFeatures(ObservationTaskResult observationTask, Satellite satellite, double t0, double t1)
    {
        var radarAngle = satellite.RadarAngleDeg;
        var lookAngle = satellite.LookAngleDeg;
        var orbit = satellite.ToOrbit();
        var node = observationTask.Node + 1;
        var direction = observationTask.Direction.ToString();

        var (baseNear, baseFar) = orbit.BuildSwaths2(node - 1, t0 - 60, t1 + 60, 10, lookAngle, radarAngle, Enum.Parse<SpaceScience.Model.SwathDirection>(direction));
        var baseNear2 = LonSplitters.Default.Split(baseNear);
        var baseFar2 = LonSplitters.Default.Split(baseFar);

        var baseNearSwathFeature = FeatureBuilder.CreateTrack(baseNear2, style: StyleBuilder.CreateFootprintSwathStyle());

        var baseFarSwathFeature = FeatureBuilder.CreateTrack(baseFar2, style: StyleBuilder.CreateFootprintSwathStyle());

        var (near, far) = orbit.BuildSwaths2(node - 1, t0, t1, 2, lookAngle, radarAngle, Enum.Parse<SpaceScience.Model.SwathDirection>(direction));

        var firstNearPoint = near.First();
        var lastNearPoint = near.Last();
        var lastFarPoint = far.Last();
        var firstFarPoint = far.First();

        var res = LonSplitters.Default.SplitArea(new() { firstNearPoint, lastNearPoint, lastFarPoint, firstFarPoint });

        var geoms = res
            .Select(s => s.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg)).ToList())
            .Select(s => s.ToClosedCoordinates())
            .Select(s => new LinearRing(s))
            .Select(s => (Geometry)new Polygon(s));

        var area = new GeometryFactory()
            .BuildGeometry(geoms)
            .ToFeatureEx(style: StyleBuilder.CreateFootprintSwathAreaStyle());

        var features = new List<IFeature>
        {
            baseNearSwathFeature,
            baseFarSwathFeature,
            area
        };

        return features;
    }

    private IFeature GetGroundTargetFeature(ObservationTaskResult observationTask)
    {
        var targetName = observationTask.TargetName;

        var groundTarget = _gts.Where(s => Equals(s.Name, targetName)).FirstOrDefault()!;

        var feature = FeatureBuilder.CreateGroundTarget(groundTarget);

        if (groundTarget.Type == GroundTargetType.Point)
        {
            feature.Styles.Add(StyleBuilder.CreateGroundTargetPointStyle());
        }
        else
        {
            feature.Styles.Add(StyleBuilder.CreateGroundTargetStyle());
        }

        return feature;
    }
}
