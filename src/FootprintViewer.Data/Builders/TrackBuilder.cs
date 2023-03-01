using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using SpaceScience;

namespace FootprintViewer.Data.Builders;

public static class TrackBuilder
{
    public static IDictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>> Create(IEnumerable<Satellite> satellites)
    {
        var tracks = new Dictionary<string, Dictionary<int, List<List<(double lon, double lat)>>>>();

        foreach (var satellite in satellites)
        {
            tracks.Add(satellite.Name!, BuildTracks(satellite));
        }

        return tracks;
    }

    private static Dictionary<int, List<List<(double, double)>>> BuildTracks(Satellite satellite)
    {
        var tracks = new Dictionary<int, List<List<(double, double)>>>();

        var sat = satellite.ToPRDCTSatellite();

        foreach (var node in sat.Nodes())
        {
            var coords = sat.GetGroundTrackDynStep(node.Value - 1, 60.0, ScienceConverters.From180To180);

            tracks.Add(node.Value, new List<List<(double, double)>>());

            var temp = new List<(double, double)>();

            Geo2D old = coords[0], cur;
            for (int i = 0; i < coords.Count; i++)
            {
                cur = coords[i];

                if (Math.Abs(cur.Lon - old.Lon) >= 3.2)
                {
                    double cutLat = LinearInterpDiscontLat(old, cur);

                    if (old.Lon > 0.0)
                    {
                        temp.Add((180.0, cutLat * 180.0 / Math.PI));
                        tracks[node.Value].Add(temp);
                        temp = new()
                        {
                            (-180.0, cutLat * 180.0 / Math.PI),
                            (cur.ToDegrees().Lon, cur.ToDegrees().Lat)
                        };
                    }
                    else
                    {
                        temp.Add((-180.0, cutLat * 180.0 / Math.PI));
                        tracks[node.Value].Add(temp);
                        temp = new()
                        {
                            (180.0, cutLat * 180.0 / Math.PI),
                            (cur.ToDegrees().Lon, cur.ToDegrees().Lat)
                        };
                    }
                }
                else
                {
                    temp.Add((cur.ToDegrees().Lon, cur.ToDegrees().Lat));
                }

                old = cur;
            }

            tracks[node.Value].Add(temp);
        }

        return tracks;
    }

    private static double LinearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
    {
        Geo2D p1 = pp1.ToRadians(), p2 = pp2.ToRadians();

        // one longitude should be negative one positive, make them both positive
        double lon1 = p1.Lon, lat1 = p1.Lat, lon2 = p2.Lon, lat2 = p2.Lat;
        if (lon1 > lon2)
        {
            lon2 += 2 * Math.PI; // in radians
        }
        else
        {
            lon1 += 2 * Math.PI;
        }

        return (lat1 + (Math.PI - lon1) * (lat2 - lat1) / (lon2 - lon1));
    }
}
