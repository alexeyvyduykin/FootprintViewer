using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class OrbitExtensions
{
    public record class TrackBuilderResult(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Track);

    public record class SwathBuilderResult(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Left, Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Right);

    public static int NodesOnDay(this Orbit orbit)
    {
        return (int)Math.Floor(86400.0 / orbit.Period);
    }

    /// <summary>
    /// nodes = [1, 2, 3 ... n]
    /// </summary>
    public static int[] ToNodesOnDay(this Orbit orbit)
    {
        var nodeCount = orbit.NodesOnDay();
        var nodes = Enumerable.Range(1, nodeCount).ToArray();
        return nodes;
    }

    public static TrackBuilderResult BuildTracks(this Orbit orbit)
    {
        var track = new GroundTrack(orbit);

        track.CalculateTrackWithLogStep(100);

        var nodes = orbit.NodesOnDay();

        var res = Enumerable.Range(1, nodes)
            .ToDictionary(s => s, s => LonSplitters.Default.Split(track.GetTrack(s)));

        return new TrackBuilderResult(res);
    }

    public static SwathBuilderResult BuildSwaths(this Orbit orbit, double lookAngleDeg, double radarAngleDeg)
    {
        var leftSwath = new Swath(orbit, lookAngleDeg, radarAngleDeg, SwathDirection.Left);
        var rightSwath = new Swath(orbit, lookAngleDeg, radarAngleDeg, SwathDirection.Right);

        var nodeCount = orbit.NodesOnDay();
        var nodes = Enumerable.Range(1, nodeCount).ToArray();

        var leftRes = leftSwath.BuildSwaths(nodes);
        var rightRes = rightSwath.BuildSwaths(nodes);

        return new SwathBuilderResult(leftRes, rightRes);
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static (List<(double lonDeg, double latDeg)> near, List<(double lonDeg, double latDeg)> far) BuildSwaths2(this Orbit orbit, int node, double t0, double t1, double dt, double lookAngleDeg, double radarAngleDeg, SwathDirection direction)
    {
        var swath = new Swath(orbit, lookAngleDeg, radarAngleDeg, direction);

        var (near, far) = BuildSwaths2(swath, node, t0, t1, dt);

        return (near, far);
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    public static (List<(double lonDeg, double latDeg)> near, List<(double lonDeg, double latDeg)> far) BuildSwaths(this Orbit orbit, int node, double t0, double t1, double dt, double lookAngleDeg, double radarAngleDeg, SwathDirection direction)
    {
        var swath = new Swath(orbit, lookAngleDeg, radarAngleDeg, direction);

        var (near, far) = BuildSwaths(swath, node, t0, t1, dt);

        return (near, far);
    }

    public static (double centralAngleMinDeg, double centralAngleMaxDeg) GetValidRange(this Orbit orbit, double angle1Deg, double angle2Deg)
    {
        double u0 = 0;
        double a = orbit.SemimajorAxis;
        double ecc = orbit.Eccentricity;
        double w = orbit.ArgumentOfPerigee;
        double z1 = (90.0 - angle1Deg) * SpaceMath.DegreesToRadians; //  gam1=20
        double z2 = (90.0 - angle2Deg) * SpaceMath.DegreesToRadians; //  gam2=55

        var gam1 = Math.PI / 2 - z1;
        var gam2 = Math.PI / 2 - z2;
        var ph = a * (1 - ecc * ecc);
        var H = ph / (1 + ecc * Math.Cos(u0 - w)) - Constants.Re;

        double centralAngleMinDeg = (Math.PI / 2 - gam1 - Math.Acos((Constants.Re + H) * Math.Sin(gam1) / Constants.Re)); //2 центр.угол мин.огр полосы обзора
        double centralAngleMaxDeg = (Math.PI / 2 - gam2 - Math.Acos((Constants.Re + H) * Math.Sin(gam2) / Constants.Re)); //2 центр.угол макс.огр.полосы обзора

        centralAngleMinDeg *= SpaceMath.RadiansToDegrees;
        centralAngleMaxDeg *= SpaceMath.RadiansToDegrees;

        return (centralAngleMinDeg, centralAngleMaxDeg);
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    private static (List<(double lonDeg, double latDeg)> near, List<(double lonDeg, double latDeg)> far) BuildSwaths(Swath swath, int node, double t0, double t1, double dt)
    {
        swath.CalculateSwathOnInterval(t0, t1, dt);

        var near = swath.GetNearTrack(node, t1 - t0, LonConverters.Default);

        var far = swath.GetFarTrack(node, t1 - t0, LonConverters.Default);

        return (near, far);
    }

    /// <summary>
    /// node = [1; n]
    /// </summary>
    private static (List<(double lonDeg, double latDeg)> near, List<(double lonDeg, double latDeg)> far) BuildSwaths2(Swath swath, int node, double t0, double t1, double dt)
    {
        swath.CalculateSwathOnInterval(t0, t1, dt);

        var near = swath.GetNearTrack(node, t1 - t0);

        var far = swath.GetFarTrack(node, t1 - t0);

        return (near, far);
    }
}
