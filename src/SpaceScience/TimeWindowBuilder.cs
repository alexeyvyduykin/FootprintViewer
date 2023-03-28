using SpaceScience.Extensions;
using SpaceScience.Model;

namespace SpaceScience;

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
    public IList<TimeWindowResult> BuildOnNode(PRDCTSatellite satellite, int node, double angle1Deg, double angle2Deg, List<(double lon, double lat, string obj_name)> targets)
    {
        return BuildOnNodes(satellite, node, node, angle1Deg, angle2Deg, targets);
    }

    public IList<TimeWindowResult> BuildOnNodes(PRDCTSatellite satellite, int fromNode, int toNode, double angle1Deg, double angle2Deg, List<(double lon, double lat, string obj_name)> targets)
    {
        var orbit = satellite.Orbit;

        var (centralAngleMinDeg, centralAngleMaxDeg) = orbit.GetValidRange(angle1Deg, angle2Deg);

        var list = new List<TimeWindowResult>();

        var dt = 1.0;
        var beginSkip = (int)(10.0 / dt);
        var endSkip = (int)(10.0 / dt);

        var track = new GroundTrack(orbit);

        var factor = new FactorShiftTrack(orbit, angle1Deg, angle2Deg, SwathDirection.Left);
        var nearTrack = new GroundTrack(orbit, factor, angle1Deg, TrackDirection.Left);
        var farTrack = new GroundTrack(orbit, factor, angle2Deg, TrackDirection.Left);

        track.CalculateTrack(dt);
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
            double uBeginVisible = double.MinValue;
            double tBeginVisible = double.MinValue;
            double uEndVisible = double.MaxValue;
            double tEndVisible = double.MaxValue;

            for (int node = fromNode; node <= toNode; node++)
            {
                var fullTrack = track.GetFullTrack(node, LonConverters.Default).Select((s, index) => (s, index));

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
                    double centralAngleDeg = SpaceMethods.CreateCentralAngle((lonDeg, latDeg), (lonTargetDeg, latTargetDeg));

                    if (centralAngleDeg < centralAngleMinDeg)
                    {
                        // для исключение точек попавших в зону между полосами                   
                        isMiss = true;
                    }

                    isVisible = false;

                    if (centralAngleDeg <= centralAngleMaxDeg
                        && centralAngleDeg >= centralAngleMinDeg)
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

                            leftSaveDeg = nearTrack.GetTrackOfIndex(i, node, LonConverters.Default);
                            rightSaveDeg = farTrack.GetTrackOfIndex(i, node, LonConverters.Default);
                            tVis = t;
                            uVis = u;
                            lonSaveDeg = lonDeg;
                            latSaveDeg = latDeg;
                        }

                        if (counter == 1)
                        {
                            tBeginVisible = t;
                            uBeginVisible = u;
                        }

                        tEndVisible = t;
                        uEndVisible = u;
                    }

                    if (isVisible == false && isTemp == true)
                    {
                        if (lastCounter != 1 && isMiss == false)
                        {
                            var rLev = SpaceMethods.CreateCentralAngle(leftSaveDeg, (lonTargetDeg, latTargetDeg));
                            var rPrav = SpaceMethods.CreateCentralAngle(rightSaveDeg, (lonTargetDeg, latTargetDeg));

                            var isLeftSwath = (rLev < rPrav);

                            var track33 = new GroundTrack(satellite.Orbit);
                            track33.CalculateTrack(uBeginVisible, uEndVisible);

                            var interval = track33.GetTrack(node, LonConverters.Default);

                            var direction = new List<(double, double)>() { (lonSaveDeg, latSaveDeg), (lonTargetDeg, latTargetDeg) };

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
                                Interval = interval.ToCutList(),
                                Direction = direction.ToCutList()
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
                    var rLev = SpaceMethods.CreateCentralAngle(leftSaveDeg, (lonTargetDeg, latTargetDeg));
                    var rPrav = SpaceMethods.CreateCentralAngle(rightSaveDeg, (lonTargetDeg, latTargetDeg));

                    var isLeftSwath = (rLev < rPrav);

                    var track33 = new GroundTrack(satellite.Orbit);
                    track33.CalculateTrack(uBeginVisible, uEndVisible);

                    var interval = track33.GetTrack(toNode, LonConverters.Default);

                    var direction = new List<(double, double)>() { (lonSaveDeg, latSaveDeg), (lonTargetDeg, latTargetDeg) };

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
                        Interval = interval.ToCutList(),
                        Direction = direction.ToCutList()
                    });
                }
            }
        }

        return list;
    }
}
