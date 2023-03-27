using Avalonia.Controls;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling.Layers;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using SpaceScienceSample.Models;
using SpaceScienceSample.Samples;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpaceScienceSample;

internal class MapFactory
{
    public Map CreateMap()
    {
        var map = new Map()
        {
            CRS = "EPSG:3857",
            //   Transformation = new MinimalTransformation(),
        };

        var path = @"..\FootprintViewer\data\world\world.mbtiles";

        if (Design.IsDesignMode == false)
        {
            path = @"..\..\..\..\..\" + path;
        }

        map.Layers.Add(CreateWorldMapLayer(path));

        _ = new Sample4(map);

        return map;
    }

    public List<PlotItem> CreatePlotValues(double targetLonDeg, double targetLatDeg)
    {
        double a = 6948.0;
        double incl = 97.65;
        int node = 1;
        double lookAngleDeg = 40.0;
        double radarAngleDeg = 16.0;
        double gam1Deg = lookAngleDeg - radarAngleDeg / 2.0; // 32.0; 
        double gam2Deg = lookAngleDeg + radarAngleDeg / 2.0; // 48.0; 

        (double, double) targetDeg = (targetLonDeg, targetLatDeg);

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);

        var track = new GroundTrack(orbit);

        track.CalculateFullTrack(1.0);

        var list = new List<PlotItem>();

        foreach (var (lonDeg, latDeg, u, t) in track.GetFullTrack(node))
        {
            var res2 = TimeWindowBuilder.CreateCentralAngle((lonDeg, latDeg), targetDeg);

            list.Add(new PlotItem()
            {
                Time = t,
                Angle = res2,
                ArgumentOfLatitude = u * SpaceMath.RadiansToDegrees
            });
        }


        var watch = Stopwatch.StartNew();

        var builder = new TimeWindowBuilder();
        var res = builder.BuildOnNode(satellite, node, gam1Deg, gam2Deg, new() { (targetLonDeg, targetLatDeg, "1") });

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        return list;
    }

    public (List<PlotPoint> point, string info) Method1(double targetLonDeg, double targetLatDeg)
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

        var watch = Stopwatch.StartNew();

        var builder = new TimeWindowBuilder();
        var res = builder.BuildOnNode(satellite, node, gam1Deg, gam2Deg, new() { (targetLonDeg, targetLatDeg, "1") });

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var res0 = res.FirstOrDefault();

        var point = new PlotPoint();

        if (res0 != null)
        {
            if (res0.MinAngle >= 0.0)
            {
                point = new PlotPoint()
                {
                    Time = res0.NadirTime,
                    Angle = res0.MinAngle
                };
            }
        }

        return (new() { point }, $"Method1: {elapsedMs} ms");
    }

    public (List<PlotPoint> point, string info) Method2(double targetLonDeg, double targetLatDeg)
    {
        double a = 6948.0;
        double incl = 97.65;
        int node = 1;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);

        var watch = Stopwatch.StartNew();
        var res = Gss(satellite, node, (targetLonDeg, targetLatDeg));
        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        var res0 = res.FirstOrDefault();

        var point = new PlotPoint();

        if (res0 != null)
        {
            if (res0.MinAngle >= 0.0)
            {
                point = new PlotPoint()
                {
                    Time = res0.NadirTime,
                    Angle = res0.MinAngle
                };
            }
        }

        return (new() { point }, $"Method2: {elapsedMs} ms");
    }

    public List<PlotItem> CreatePlotAsymptotes(double lookAngleDeg = 40.0, double radarAngleDeg = 16.0)
    {
        double a = 6948.0;
        double incl = 97.65;
        double gam1Deg = lookAngleDeg - radarAngleDeg / 2.0; // 32.0; 
        double gam2Deg = lookAngleDeg + radarAngleDeg / 2.0; // 48.0; 

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var period = orbit.Period;
        var satellite = factory.CreateSatellite(orbit);

        var builder = new TimeWindowBuilder();
        var res = builder.BuildValidConus(satellite, gam1Deg, gam2Deg);

        return new()
        {
            new PlotItem() { Time = 0, MinAngle = res.angle1, MaxAngle = res.angle2 },
            new PlotItem() { Time = period, MinAngle = res.angle1, MaxAngle = res.angle2 }
        };
    }

    private static ILayer CreateWorldMapLayer(string path)
    {
        var mbTilesTileSource = new MbTilesTileSource(new SQLiteConnectionString(path, true));

        return new TileLayer(mbTilesTileSource);
    }

    public IList<TimeWindowResult> Gss(PRDCTSatellite satellite, int node, (double lonTargetDeg, double latTargetDeg) target)
    {
        var orbit = satellite.Orbit;
        var period = orbit.Period;
        var list = new List<TimeWindowResult>();

        var dt = 1.0;

        var track = new GroundTrack(orbit);

        track.CalculateFullTrack(dt);

        GoldenSectionSearch gss = new();
        var tOpt = gss.Search(func, 0.0, period);

        double dltob = func(tOpt);
        var uOpt = orbit.Anomalia(tOpt);

        list.Add(new TimeWindowResult()
        {
            MinAngle = dltob,
            NadirTime = tOpt
        });

        return list;

        double func(double t)
        {
            var u = orbit.Anomalia(t);

            var (lonDeg, latDeg) = track.ContinuousTrack22(u);

            lonDeg = GetTrack(track, lonDeg, node, LonConverter);

            double dltob = CreateCentralAngle((lonDeg, latDeg), (target.lonTargetDeg, target.latTargetDeg));

            return dltob;
        }
    }

    public double GetTrack(GroundTrack track, double lonDeg, int node, Func<double, double>? lonConverter = null)
    {
        var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * node;

        if (lonConverter != null)
        {
            return lonConverter.Invoke(lonDeg + offset);
        }

        return lonDeg + offset;
    }

    public static double CreateCentralAngle((double lonDeg, double latDeg) trackPoint, (double lonDeg, double latDeg) target)
    {
        double R = 6371.110;

        var lonRad = trackPoint.lonDeg * SpaceMath.DegreesToRadians;
        var latRad = trackPoint.latDeg * SpaceMath.DegreesToRadians;

        var targetLonRad = target.lonDeg * SpaceMath.DegreesToRadians;
        var targetLatRad = target.latDeg * SpaceMath.DegreesToRadians;

        //(X, Y, Z) - Координаты подспутниковой точки
        double X = R * Math.Cos(lonRad);
        double Y = R * Math.Sin(lonRad);
        double Z = R * Math.Sin(latRad) / Math.Sqrt(1 - Math.Sin(latRad) * Math.Sin(latRad));
        //(x, y, z) - Координаты объекта наблюдения
        double x = R * Math.Cos(targetLonRad);
        double y = R * Math.Sin(targetLonRad);
        double z = R * Math.Sin(targetLatRad) / Math.Sqrt(1 - Math.Sin(targetLatRad) * Math.Sin(targetLatRad));

        return Math.Acos((x * X + y * Y + z * Z) / (Math.Sqrt(x * x + y * y + z * z) * Math.Sqrt(X * X + Y * Y + Z * Z))) * SpaceMath.RadiansToDegrees;
    }

    private static double LonConverter(double lonDeg)
    {
        while (lonDeg > 180) lonDeg -= 360.0;
        while (lonDeg < -180) lonDeg += 360.0;
        return lonDeg;
    }
}