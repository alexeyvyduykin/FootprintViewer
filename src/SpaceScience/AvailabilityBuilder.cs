using SpaceScience.Extensions;
using SpaceScience.Model;

namespace SpaceScience;

public class AvailabilityBuilderResult
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
    public List<List<(double lonDeg, double latDeg)>> Interval { get; set; } = new();
    public List<List<(double lonDeg, double latDeg)>> Direction { get; set; } = new();
}

public class AvailabilityBuilder
{
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheTrack = new();
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheLeft = new();
    private readonly Dictionary<int, IList<(double t, double u, double lon, double lat)>> _cacheRight = new();

    private static double PI = 3.14159265358979323846;
    private readonly double GR = 180.0 / PI;
    private readonly double R = 6371.110;

    private static double sin(double value) => Math.Sin(value);
    private static double cos(double value) => Math.Cos(value);
    private static double acos(double value) => Math.Acos(value);
    private static double sqrt(double value) => Math.Sqrt(value);
    private static bool MinLon(double lon) => lon < -PI;
    private static bool MaxLon(double lon) => lon > PI;

    public IList<AvailabilityBuilderResult> BuildSample()
    {
        double a = 6948.0;
        double incl = 97.65;

        var factory = new SpaceScienceFactory();
        var orbit = factory.CreateOrbit(a, incl);
        var satellite = factory.CreateSatellite(orbit);

        var builder = new AvailabilityBuilder();
        var random = new Random();

        var targets = new List<(double, double, string)>();

        for (int i = 0; i < 100; i++)
        {
            var lat = (double)random.Next(-80, 81);
            var lon = (double)random.Next(0, 360);

            targets.Add((lon, lat, $"{i + 1}"));
        }

        return builder.Build(satellite, 20.0, 55.0, targets);
    }

    public IList<AvailabilityBuilderResult> Build(PRDCTSatellite satellite, double angle1Deg, double angle2Deg, IList<(double lon, double lat, string obj_name)> targets)
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
        var H = ph / (1 + ecc * cos(u0 - w)) - R;

        double dlt1 = (PI / 2 - gam1 - acos((R + H) * sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double dlt2 = (PI / 2 - gam2 - acos((R + H) * sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора
        double dltpredel = acos(R / (R + H)) * GR;//2 центр угол (вроде не нужен, но хз)

        var list = new List<AvailabilityBuilderResult>();

        Cache(satellite, angle1Deg, 1.0);

        var nodeCount = satellite.Nodes().Count;

        foreach (var (lonTarget, latTarget, OBJ_N) in targets)
        {
            double L = lonTarget;

            double dltka1 = dlt1;
            double dltka2 = dlt2;
            while (L > PI) L -= 2 * PI;
            while (L < -PI) L += 2 * PI;

            double l1LEV = 0, m1LEV = 0, l1PR = 0, m1PR = 0;
            double tVis = double.NaN;
            double tBeginVis = double.MinValue;
            double tEndVis = double.MaxValue;
            double uVis = double.NaN;
            double uBeginVis = double.MinValue;
            double uEndVis = double.MaxValue;
            for (int i = 0; i < nodeCount; i++)
            {
                double rMin = 10000;

                int COUNTER = 0;

                var jCount = _cacheTrack[i].Count;

                for (int j = 0; j < jCount; j++)
                {
                    var (t, u, lon, lat) = _cacheTrack[i][j];
                    var (_, _, lonLeft, latLeft) = _cacheLeft[i][j];
                    var (_, _, lonRight, latRight) = _cacheRight[i][j];

                    //(X, Y, Z) - Координаты подспутниковой точки
                    double X = R * cos(lon);
                    double Y = R * sin(lon);
                    double Z = R * sin(lat) / sqrt(1 - sin(lat) * sin(lat));
                    //(x, y, z) - Координаты объекта наблюдения
                    double x = R * cos(L);
                    double y = R * sin(L);
                    double z = R * sin(latTarget) / sqrt(1 - sin(latTarget) * sin(latTarget));

                    double dltob = acos((x * X + y * Y + z * Z) / (sqrt(x * x + y * y + z * z) * sqrt(X * X + Y * Y + Z * Z))) * GR;//2
                    if (dltob < dltka1)
                    {
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
                            dltob = rMin;
                            tVis = t;
                            uVis = u;
                            l1LEV = latLeft;
                            m1LEV = lonLeft;
                            l1PR = latRight;
                            m1PR = lonRight;
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

                if (COUNTER > 0)
                {
                    double xob = R * cos(L);
                    double yob = R * sin(L);
                    double zob = R * sin(latTarget) / sqrt(1 - sin(latTarget) * sin(latTarget));
                    double xLev = R * cos(m1LEV);
                    double yLev = R * sin(m1LEV);
                    double zLev = R * sin(l1LEV) / sqrt(1 - sin(l1LEV) * sin(l1LEV));
                    double xPrav = R * cos(m1PR);
                    double yPrav = R * sin(m1PR);
                    double zPrav = R * sin(l1PR) / sqrt(1 - sin(l1PR) * sin(l1PR));
                    var rLev = acos((xob * xLev + yob * yLev + zob * zLev) / (sqrt(xob * xob + yob * yob + zob * zob) * sqrt(xLev * xLev + yLev * yLev + zLev * zLev))) * GR;//2
                    var rPrav = acos((xob * xPrav + yob * yPrav + zob * zPrav) / (sqrt(xob * xob + yob * yob + zob * zob) * sqrt(xPrav * xPrav + yPrav * yPrav + zPrav * zPrav))) * GR;//2

                    var isLeftSwath = (rLev < rPrav);

                    list.Add(new()
                    {
                        Name = OBJ_N,
                        Lat = latTarget,
                        Lon = L,
                        Node = i + 1,
                        IsLeftSwath = isLeftSwath,
                        NadirTime = tVis,
                        BeginTime = tBeginVis,
                        EndTime = tEndVis,
                        NadirU = uVis,
                        BeginU = uBeginVis,
                        EndU = uEndVis
                    });
                }
            }
        }

        return list;
    }

    public IList<AvailabilityBuilderResult> BuildOnNode(PRDCTSatellite satellite, int node, double angle1Deg, double angle2Deg, IList<(double lon, double lat, string obj_name)> targets)
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
        var H = ph / (1 + ecc * cos(u0 - w)) - R;

        double dlt1 = (PI / 2 - gam1 - acos((R + H) * sin(gam1) / R)) * GR; //2 центр.угол мин.огр полосы обзора
        double dlt2 = (PI / 2 - gam2 - acos((R + H) * sin(gam2) / R)) * GR; //2 центр.угол макс.огр.полосы обзора
        double dltpredel = acos(R / (R + H)) * GR;//2 центр угол (вроде не нужен, но хз)

        var list = new List<AvailabilityBuilderResult>();

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

            double rMin = 10000;
            int counterSave = 0;
            int COUNTER = 0;

            foreach (var ((lonDeg, latDeg, u, t), i) in track.GetFullTrack(node, LonConverter).Select((s, index) => (s, index)))
            {
                double dltob = CreateCentralAngle((lonDeg, latDeg), (lonTargetDeg, latTargetDeg));

                if (dltob < dltka1)
                {
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

            if (COUNTER > 0 && counterSave != 1 && counterSave != COUNTER)
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
                    Interval = interval.ToCutList(),
                    Direction = direction.ToCutList()
                });
            }

        }

        return list;
    }

    private static double CreateCentralAngle((double lonDeg, double latDeg) trackPoint, (double lonDeg, double latDeg) target)
    {
        double R = 6371.110;

        var lonRad = trackPoint.lonDeg * SpaceMath.DegreesToRadians;
        var latRad = trackPoint.latDeg * SpaceMath.DegreesToRadians;

        var targetLonRad = target.lonDeg * SpaceMath.DegreesToRadians;
        var targetLatRad = target.latDeg * SpaceMath.DegreesToRadians;

        //(X, Y, Z) - Координаты подспутниковой точки
        double X = R * cos(lonRad);
        double Y = R * sin(lonRad);
        double Z = R * sin(latRad) / sqrt(1 - sin(latRad) * sin(latRad));
        //(x, y, z) - Координаты объекта наблюдения
        double x = R * cos(targetLonRad);
        double y = R * sin(targetLonRad);
        double z = R * sin(targetLatRad) / sqrt(1 - sin(targetLatRad) * sin(targetLatRad));

        return acos((x * X + y * Y + z * Z) / (sqrt(x * x + y * y + z * z) * sqrt(X * X + Y * Y + Z * Z))) * SpaceMath.RadiansToDegrees;
    }

    private static double LonConverter(double lonDeg)
    {
        while (lonDeg > 180) lonDeg -= 360.0;
        while (lonDeg < -180) lonDeg += 360.0;
        return lonDeg;
    }

    private void Cache(PRDCTSatellite satellite, double angle = 20.0, double dt = 1.0)
    {
        _cacheTrack.Clear();
        _cacheLeft.Clear();
        _cacheRight.Clear();

        var orbit = satellite.Orbit;
        var nodeCount = satellite.Nodes().Count;
        var period = orbit.Period;

        for (int i = 0; i < nodeCount; i++)
        {
            _cacheTrack.Add(i, new List<(double t, double u, double lon, double lat)>());
            _cacheLeft.Add(i, new List<(double t, double u, double lon, double lat)>());
            _cacheRight.Add(i, new List<(double t, double u, double lon, double lat)>());

            for (double t = 0; t <= period; t += dt)
            {
                var u = orbit.Anomalia(t);

                var (lon, lat) = new CustomTrack(satellite.Orbit, 0.0, TrackPointDirection.None)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                var (lonLeft, latLeft) = new CustomTrack(satellite.Orbit, angle, TrackPointDirection.Left)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                var (lonRight, latRight) = new CustomTrack(satellite.Orbit, angle, TrackPointDirection.Right)
                    .TrackPoint(u)
                    .ToLonRange(MinLon, MaxLon);

                _cacheTrack[i].Add((t, u, lon, lat));
                _cacheLeft[i].Add((t, u, lonLeft, latLeft));
                _cacheRight[i].Add((t, u, lonRight, latRight));
            }
        }

        return;
    }
}
