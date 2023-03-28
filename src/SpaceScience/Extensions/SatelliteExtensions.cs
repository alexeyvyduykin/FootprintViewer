using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class SatelliteExtensions
{
    public record class TrackBuilderResult(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Track);

    public record class SwathBuilderResult(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Left, Dictionary<int, List<List<(double lonDeg, double latDeg)>>> Right);

    public static TrackBuilderResult BuildTracks(this PRDCTSatellite satellite)
    {
        var track = new GroundTrack(satellite.Orbit);

        track.CalculateTrackWithLogStep(100);

        var res = satellite
            .Nodes()
            .Select((_, index) => index)
            .ToDictionary(s => s, s => track.GetTrack(s, LonConverters.Default).ToCutList());

        return new TrackBuilderResult(res);
    }

    public static SwathBuilderResult BuildSwaths(this PRDCTSatellite satellite, double lookAngleDeg, double radarAngleDeg)
    {
        var leftSwath = new Swath(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathDirection.Left);
        var rightSwath = new Swath(satellite.Orbit, lookAngleDeg, radarAngleDeg, SwathDirection.Right);

        var leftRes = BuildSwaths(satellite, leftSwath);
        var rightRes = BuildSwaths(satellite, rightSwath);

        return new SwathBuilderResult(leftRes, rightRes);
    }

    private static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildSwaths(PRDCTSatellite satellite, Swath swath)
    {
        var swaths = new Dictionary<int, List<List<(double lonDeg, double latDeg)>>>();

        var nodes = satellite.Nodes().Count;

        swath.CalculateSwathWithLogStep();

        for (int i = 0; i < nodes; i++)
        {
            var near = swath
                .GetNearTrack(i, LonConverters.Default)
                .Select(s => (s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var far = swath
                .GetFarTrack(i, LonConverters.Default)
                .Select(s => (s.lonDeg * SpaceMath.DegreesToRadians, s.latDeg * SpaceMath.DegreesToRadians))
                .ToList();

            var engine2D = new SwathCore2D(near, far, swath.IsCoverPolis);

            var shapes = engine2D.CreateShapes(false, false);

            var convertShapes = shapes.Select(s => s.Select(g => (g.lonRad * SpaceMath.RadiansToDegrees, g.latRad * SpaceMath.RadiansToDegrees)).ToList()).ToList();

            swaths.Add(i, convertShapes);
        }

        return swaths;
    }
}
