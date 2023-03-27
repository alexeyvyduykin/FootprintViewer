using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceScienceSample.Models;

public class TimeWindowResult
{
    public string Name { get; set; } = null!;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public int Node { get; set; }
    public bool IsLeftSwath { get; set; }
    public double NadirTime { get; set; }
    public double BeginTime { get; set; }
    public double EndTime { get; set; }
    public double NadirU { get; set; }
    public double BeginU { get; set; }
    public double EndU { get; set; }
    public double MinAngle { get; set; }
    public List<List<(double lonDeg, double latDeg)>> Interval { get; set; } = new();
    public List<List<(double lonDeg, double latDeg)>> Direction { get; set; } = new();
}

public class TimeWindowBuilder
{
    private static double PI = 3.14159265358979323846;
    private readonly double GR = 180.0 / PI;
    private readonly double R = 6371.110;

    public (double angle1, double angle2) BuildValidConus(PRDCTSatellite satellite, double angle1Deg, double angle2Deg)
    {
        var orbit = satellite.Orbit;
        double u0 = 0;
        double a = orbit.SemimajorAxis;
        double ecc = orbit.Eccentricity;
        double w = orbit.ArgumentOfPerigee;
        double z1 = (90.0 - angle1Deg) * SpaceMath.DegreesToRadians; //  gam1=20
        double z2 = (90.0 - angle2Deg) * SpaceMath.DegreesToRadians; //  gam2=55

        var gam1 = PI / 2 - z1;
        var gam2 = PI / 2 - z2;
        var ph = a * (1 - ecc * ecc);
        var H = ph / (1 + ecc * Math.Cos(u0 - w)) - R;

        double dlt1 = (PI / 2 - gam1 - Math.Acos((R + H) * Math.Sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double dlt2 = (PI / 2 - gam2 - Math.Acos((R + H) * Math.Sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора

        return (dlt1, dlt2);
    }

    public IList<TimeWindowResult> BuildOnNode(PRDCTSatellite satellite, int node, double angle1Deg, double angle2Deg, List<(double lon, double lat, string obj_name)> targets)
    {
        var orbit = satellite.Orbit;
        double u0 = 0;
        double a = orbit.SemimajorAxis;
        double ecc = orbit.Eccentricity;
        double w = orbit.ArgumentOfPerigee;
        double z1 = (90.0 - angle1Deg) * SpaceMath.DegreesToRadians; //  gam1=20
        double z2 = (90.0 - angle2Deg) * SpaceMath.DegreesToRadians; //  gam2=55

        var gam1 = PI / 2 - z1;
        var gam2 = PI / 2 - z2;
        var ph = a * (1 - ecc * ecc);
        var H = ph / (1 + ecc * Math.Cos(u0 - w)) - R;

        double dlt1 = (PI / 2 - gam1 - Math.Acos((R + H) * Math.Sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double dlt2 = (PI / 2 - gam2 - Math.Acos((R + H) * Math.Sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора
        double dltpredel = Math.Acos(R / (R + H)) * GR;//2 центр угол (вроде не нужен, но хз)

        var list = new List<TimeWindowResult>();

        var dt = 1.0;

        var track = new GroundTrack(orbit);

        var factor = new FactorShiftTrack(orbit, angle1Deg, angle2Deg, SwathMode.Left);
        var nearTrack = new GroundTrack(orbit, factor, angle1Deg, TrackPointDirection.Left);
        var farTrack = new GroundTrack(orbit, factor, angle2Deg, TrackPointDirection.Left);

        track.CalculateFullTrack(dt);
        nearTrack.CalculateTrack(dt);
        farTrack.CalculateTrack(dt);

        foreach (var (lonTargetDeg, latTargetDeg, OBJ_N) in targets)
        {
            double dltka1 = dlt1;
            double dltka2 = dlt2;

            (double lon, double lat) leftSaveDeg = (0.0, 0.0);
            (double lon, double lat) rightSaveDeg = (0.0, 0.0);
            double lonSaveDeg = 0.0;
            double latSaveDeg = 0.0;
            double tVis = double.NaN;
            double tBeginVis = double.MinValue;
            double tEndVis = double.MaxValue;
            double uVis = double.NaN;
            double uBeginVis = double.MinValue;
            double uEndVis = double.MaxValue;

            double rMin = double.MaxValue;
            int counterSave = 0;
            int COUNTER = 0;

            foreach (var ((lonDeg, latDeg, u, t), i) in track.GetFullTrack(node, LonConverter).Select((s, index) => (s, index)))
            {
                double dltob = CreateCentralAngle((lonDeg, latDeg), (lonTargetDeg, latTargetDeg));

                if (dltob < dltka1)
                {
                    // для исключение точек попавших в зону между полосами
                    COUNTER = -1000;
                }

                if (dltob <= dltka2
                    && dltob >= dltka1
                    && dltob <= dltpredel
                    && dltob >= (-dltpredel))
                {
                    COUNTER++;
                    if (dltob < rMin)
                    {
                        counterSave = COUNTER;
                        leftSaveDeg = nearTrack.CacheTrack[i];
                        rightSaveDeg = farTrack.CacheTrack[i];
                        rMin = dltob;
                        tVis = t;
                        uVis = u;
                        lonSaveDeg = lonDeg;
                        latSaveDeg = latDeg;
                    }
                    if (COUNTER == 1)
                    {
                        tBeginVis = t;
                        uBeginVis = u;
                    }
                    tEndVis = t;
                    uEndVis = u;
                }
            }

            if (COUNTER > 0 /*&& counterSave != 1 && counterSave != COUNTER*/)
            {
                var rLev = CreateCentralAngle(leftSaveDeg, (lonTargetDeg, latTargetDeg));
                var rPrav = CreateCentralAngle(rightSaveDeg, (lonTargetDeg, latTargetDeg));

                var isLeftSwath = (rLev < rPrav);

                var track33 = new GroundTrack(satellite.Orbit);
                track33.CalculateTrack(uBeginVis, uEndVis);

                var interval = track33.GetTrack(node, LonConverter);

                var direction = new List<(double, double)>() { (lonSaveDeg, latSaveDeg), (lonTargetDeg, latTargetDeg) };

                list.Add(new()
                {
                    Name = OBJ_N,
                    Lat = latTargetDeg,
                    Lon = lonTargetDeg,
                    Node = node,
                    IsLeftSwath = isLeftSwath,
                    NadirTime = tVis,
                    BeginTime = tBeginVis,
                    EndTime = tEndVis,
                    NadirU = uVis,
                    BeginU = uBeginVis,
                    EndU = uEndVis,
                    MinAngle = rMin,
                    Interval = interval.ToCutList(),
                    Direction = direction.ToCutList()
                });
            }

        }

        return list;
    }

    public IList<TimeWindowResult> BuildOnNodes(PRDCTSatellite satellite, int fromNode, int toNode, double angle1Deg, double angle2Deg, List<(double lon, double lat, string obj_name)> targets)
    {
        var orbit = satellite.Orbit;
        double u0 = 0;
        double a = orbit.SemimajorAxis;
        double ecc = orbit.Eccentricity;
        double w = orbit.ArgumentOfPerigee;
        double z1 = (90.0 - angle1Deg) * SpaceMath.DegreesToRadians; //  gam1=20
        double z2 = (90.0 - angle2Deg) * SpaceMath.DegreesToRadians; //  gam2=55

        var gam1 = PI / 2 - z1;
        var gam2 = PI / 2 - z2;
        var ph = a * (1 - ecc * ecc);
        var H = ph / (1 + ecc * Math.Cos(u0 - w)) - R;

        double angleMinDeg = (PI / 2 - gam1 - Math.Acos((R + H) * Math.Sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double angleMaxDeg = (PI / 2 - gam2 - Math.Acos((R + H) * Math.Sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора
        double dltpredel = Math.Acos(R / (R + H)) * GR;//2 центр угол (вроде не нужен, но хз)

        var list = new List<TimeWindowResult>();

        var dt = 1.0;
        var beginSkip = (int)(10.0 / dt);
        var endSkip = (int)(10.0 / dt);

        var track = new GroundTrack(orbit);

        var factor = new FactorShiftTrack(orbit, angle1Deg, angle2Deg, SwathMode.Left);
        var nearTrack = new GroundTrack(orbit, factor, angle1Deg, TrackPointDirection.Left);
        var farTrack = new GroundTrack(orbit, factor, angle2Deg, TrackPointDirection.Left);

        track.CalculateFullTrack(dt);
        nearTrack.CalculateTrack(dt);
        farTrack.CalculateTrack(dt);

        foreach (var (lonTargetDeg, latTargetDeg, OBJ_N) in targets)
        {
            (double lon, double lat) leftSaveDeg = (0.0, 0.0);
            (double lon, double lat) rightSaveDeg = (0.0, 0.0);
            double lonSaveDeg = 0.0;
            double latSaveDeg = 0.0;
            double tVis = double.NaN;
            double uVis = double.NaN;

            double minCenterlAngleDeg = double.MaxValue;

            bool isVisible = false;
            bool isTemp = false;
            bool isMiss = false;
            int lastCounter = int.MinValue;
            int counter = 0;

            for (int node = fromNode; node <= toNode; node++)
            {
                var fullTrack = track.GetFullTrack(node, LonConverter).Select((s, index) => (s, index));

                if (node == fromNode)
                {
                    fullTrack = fullTrack.Skip(beginSkip).ToList();
                }

                if (node == toNode)
                {
                    fullTrack = fullTrack.SkipLast(endSkip).ToList();
                }

                foreach (var ((lonDeg, latDeg, u, t), i) in fullTrack)
                {
                    double centralAngleDeg = CreateCentralAngle((lonDeg, latDeg), (lonTargetDeg, latTargetDeg));

                    if (centralAngleDeg < angleMinDeg)
                    {
                        // для исключение точек попавших в зону между полосами                   
                        isMiss = true;
                    }

                    isVisible = false;

                    if (centralAngleDeg <= angleMaxDeg && centralAngleDeg >= angleMinDeg
                        && centralAngleDeg <= dltpredel
                        && centralAngleDeg >= (-dltpredel))
                    {
                        isVisible = true;
                    }

                    if (isVisible == true)
                    {
                        counter++;

                        isTemp = true;
                        if (centralAngleDeg < minCenterlAngleDeg)
                        {
                            lastCounter = counter;
                            minCenterlAngleDeg = centralAngleDeg;

                            leftSaveDeg = nearTrack.CacheTrack[i];
                            rightSaveDeg = farTrack.CacheTrack[i];
                            tVis = t;
                            uVis = u;
                            lonSaveDeg = lonDeg;
                            latSaveDeg = latDeg;
                        }
                    }

                    if (isVisible == false && isTemp == true)
                    {
                        if (lastCounter != 1 && isMiss == false)
                        {
                            var rLev = CreateCentralAngle(leftSaveDeg, (lonTargetDeg, latTargetDeg));
                            var rPrav = CreateCentralAngle(rightSaveDeg, (lonTargetDeg, latTargetDeg));

                            var isLeftSwath = (rLev < rPrav);

                            // var direction = new List<(double, double)>() { (lonSaveDeg, latSaveDeg), (lonTargetDeg, latTargetDeg) };

                            list.Add(new()
                            {
                                Name = OBJ_N,
                                Lat = latTargetDeg,
                                Lon = lonTargetDeg,
                                Node = node,
                                IsLeftSwath = isLeftSwath,
                                NadirTime = tVis,
                                NadirU = uVis,
                                MinAngle = minCenterlAngleDeg,
                                //Direction = direction.ToCutList()
                            });
                        }

                        isTemp = false;
                        isMiss = false;
                        counter = 0;
                        lastCounter = int.MinValue;
                        minCenterlAngleDeg = double.MaxValue;
                    }
                }
            }

            if (isVisible == true && isTemp == true)
            {
                if (counter != lastCounter && isMiss == false)
                {
                    var rLev = CreateCentralAngle(leftSaveDeg, (lonTargetDeg, latTargetDeg));
                    var rPrav = CreateCentralAngle(rightSaveDeg, (lonTargetDeg, latTargetDeg));

                    var isLeftSwath = (rLev < rPrav);

                    list.Add(new()
                    {
                        Name = OBJ_N,
                        Lat = latTargetDeg,
                        Lon = lonTargetDeg,
                        Node = toNode,
                        IsLeftSwath = isLeftSwath,
                        NadirTime = tVis,
                        NadirU = uVis,
                        MinAngle = minCenterlAngleDeg,
                    });
                }
            }
        }

        return list;
    }


    public static double CreateCentralAngle((double lonDeg, double latDeg) trackPoint, (double lonDeg, double latDeg) target)
    {
        var lonRad = trackPoint.lonDeg * SpaceMath.DegreesToRadians;
        var latRad = trackPoint.latDeg * SpaceMath.DegreesToRadians;

        var targetLonRad = target.lonDeg * SpaceMath.DegreesToRadians;
        var targetLatRad = target.latDeg * SpaceMath.DegreesToRadians;

        //(X, Y, Z) - Координаты подспутниковой точки
        double X = /*Constants.Re **/ Math.Cos(lonRad);
        double Y = /*Constants.Re **/ Math.Sin(lonRad);
        double Z = /*Constants.Re **/ Math.Sin(latRad) / Math.Sqrt(1 - Math.Sin(latRad) * Math.Sin(latRad));
        //(x, y, z) - Координаты объекта наблюдения
        double x = /*Constants.Re **/ Math.Cos(targetLonRad);
        double y = /*Constants.Re **/ Math.Sin(targetLonRad);
        double z = /*Constants.Re **/ Math.Sin(targetLatRad) / Math.Sqrt(1 - Math.Sin(targetLatRad) * Math.Sin(targetLatRad));

        return Math.Acos((x * X + y * Y + z * Z) / (Math.Sqrt(x * x + y * y + z * z) * Math.Sqrt(X * X + Y * Y + Z * Z))) * SpaceMath.RadiansToDegrees;
    }

    private static double LonConverter(double lonDeg)
    {
        while (lonDeg > 180) lonDeg -= 360.0;
        while (lonDeg < -180) lonDeg += 360.0;
        return lonDeg;
    }
}
