using System.Globalization;

namespace SpaceScience.Model;

public class Swath
{
    public Swath(Orbit orbit, double lookAngleDEG, double radarAngleDEG, SwathMode mode)
    {
        Orbit = orbit;

        (TrackPointDirection near, TrackPointDirection far) = mode switch
        {
            SwathMode.Middle => (TrackPointDirection.Left, TrackPointDirection.Right),
            SwathMode.Left => (TrackPointDirection.Left, TrackPointDirection.Left),
            SwathMode.Right => (TrackPointDirection.Right, TrackPointDirection.Right),
            _ => throw new NotImplementedException()
        };

        double minLookAngleDeg = lookAngleDEG - radarAngleDEG / 2.0;
        double maxLookAngleDeg = lookAngleDEG + radarAngleDEG / 2.0;

        var factor = new FactorShiftTrack(orbit, minLookAngleDeg, maxLookAngleDeg, mode);

        NearLine = new FactorTrack(new CustomTrack(orbit, minLookAngleDeg, near), factor);
        FarLine = new FactorTrack(new CustomTrack(orbit, maxLookAngleDeg, far), factor);
    }

    public bool IsCoverPolis(double latRAD, ref double timeFromANToPolis)
    {
        double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
        if (NearLine.PolisMod(latRAD, ref angleToPolis1) == true &&
            FarLine.PolisMod(latRAD, ref angleToPolis2) == true)
        {
            if (SpaceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
            {
                if (latRAD >= 0.0)
                {
                    timeFromANToPolis = Orbit.Quart1;
                }
                else
                {
                    timeFromANToPolis = Orbit.Quart3;
                }

                return true;
            }
        }
        return false;
    }

    public bool IsCoverPolis(double latRAD)
    {
        double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
        if (NearLine.PolisMod(latRAD, ref angleToPolis1) == true &&
            FarLine.PolisMod(latRAD, ref angleToPolis2) == true)
        {
            if (SpaceMath.InRange(SpaceMath.HALFPI, angleToPolis1, angleToPolis2))
            {
                return true;
            }
        }
        return false;
    }

    public Orbit Orbit { get; }

    public IList<Geo2D> GetNearGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
    {
        var track1 = new CustomTrack(Orbit, NearLine.Alpha1 * SpaceMath.RadiansToDegrees, NearLine.Direction);
        return GetGroundTrack(track1, satellite, node, converter);
    }

    public IList<Geo2D> GetFarGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
    {
        var track2 = new CustomTrack(Orbit, FarLine.Alpha1 * SpaceMath.RadiansToDegrees, FarLine.Direction);
        return GetGroundTrack(track2, satellite, node, converter);
    }

    private static IList<Geo2D> GetGroundTrack(CustomTrack track, PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
    {
        converter ??= SpaceConverters.From0To360;

        var points = new List<Geo2D>();

        var nodes = satellite.Nodes();
        for (int q = 0; q < nodes[node].Quarts.Count; q++)
        {
            for (double t = nodes[node].Quarts[q].TimeBegin; t <= nodes[node].Quarts[q].TimeEnd; t += 5.0)
            {
                var point = track.ContinuousTrack(node, t, satellite.TrueTimePastAN, nodes[node].Quarts[q].Quart);
                points.Add(converter.Invoke(point));
            }
        }
        return points;
    }

    public static void ToFile(string path, PRDCTSatellite satellite, PRDCTSensor sensor, int node)
    {
        var leftSwath = new Swath(satellite.Orbit, sensor.LookAngleDeg, sensor.RadarAngleDeg, SwathMode.Left);
        var rightSwath = new Swath(satellite.Orbit, sensor.LookAngleDeg, sensor.RadarAngleDeg, SwathMode.Right);

        var near1 = leftSwath.GetNearGroundTrack(satellite, node).ToList();
        var far1 = leftSwath.GetFarGroundTrack(satellite, node).ToList();

        near1.ForEach(s => { Geo2D.OffsetLeftLon(s); });
        far1.ForEach(s => { Geo2D.OffsetLeftLon(s); });

        var near2 = rightSwath.GetNearGroundTrack(satellite, node).ToList();
        var far2 = rightSwath.GetFarGroundTrack(satellite, node).ToList();

        near2.ForEach(s => { Geo2D.OffsetLeftLon(s); });
        far2.ForEach(s => { Geo2D.OffsetLeftLon(s); });

        var temp = CultureInfo.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        using (var writer = new StreamWriter(path, false))
        {
            writer.WriteLine("BEGIN OBJECT");
            writer.WriteLine("BEGIN DATA");

            for (int i = 0; i < near1.Count; i++)
            {
                writer.WriteLine("{0:0.00000000} {1:0.00000000} {2:0.00000000} {3:0.00000000} {4:0.00000000} {5:0.00000000} {6:0.00000000} {7:0.00000000}",
                    far1[i].Lon,
                    far1[i].Lat,
                    near1[i].Lon,
                    near1[i].Lat,
                    near2[i].Lon,
                    near2[i].Lat,
                    far2[i].Lon,
                    far2[i].Lat);
            }

            writer.WriteLine("END");
            writer.WriteLine("END");
        }

        Thread.CurrentThread.CurrentCulture = temp;
    }

    //public List<List<Geo2D>> To2D(PRDCTSatellite satellite, int node, bool extrude = false, bool clockwise = true)
    //{
    //    var near = GetNearGroundTrack(satellite, node).ToList();
    //    var far = GetFarGroundTrack(satellite, node).ToList();

    //    near.ForEach(s => { Geo2D.OffsetLeftLon(s); });
    //    far.ForEach(s => { Geo2D.OffsetLeftLon(s); });


    //    SwathCore2D engine2D = new SwathCore2D(near, far, IsCoverPolis);

    //    engine2D.ExtrudeMode = extrude;
    //    engine2D.CreateShapes(clockwise, out List<List<Geo2D>> shapes);

    //    return shapes;
    //}

    public FactorTrack NearLine { get; private set; }
    public FactorTrack FarLine { get; private set; }
}
