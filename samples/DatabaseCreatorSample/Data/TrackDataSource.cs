using System;
using System.Collections.Generic;

namespace DatabaseCreatorSample.Data
{
    public class TrackDataSource
    {
        private readonly IDictionary<int, IList<IList<(double lon, double lat)>>> _tracks;
        private readonly Satellite _satellite;

        public TrackDataSource(Satellite satellite)
        {
            _satellite = satellite;

            _tracks = Build(satellite);
        }

        private static Dictionary<int, IList<IList<(double, double)>>> Build(Satellite satellite)
        {
            var tracks = new Dictionary<int, IList<IList<(double, double)>>>();

            var sat = satellite.ToPRDCTSatellite();

            foreach (var node in sat.Nodes())
            {
                var coords = sat.GetGroundTrackDynStep(node.Value - 1, 60.0, ScienceConverters.From180To180);

                tracks.Add(node.Value, new List<IList<(double, double)>>());

                List<(double, double)> temp = new List<(double, double)>();

                Geo2D old = coords[0], cur;
                for (int i = 0; i < coords.Count; i++)
                {
                    cur = coords[i];

                    if (Math.Abs(cur.Lon - old.Lon) >= 3.2)
                    {
                        double cutLat = linearInterpDiscontLat(old, cur);

                        if (old.Lon > 0.0)
                        {
                            temp.Add((180.0, cutLat * 180.0 / Math.PI));
                            tracks[node.Value].Add(temp);
                            temp = new List<(double, double)>();
                            temp.Add((-180.0, cutLat * 180.0 / Math.PI));
                            temp.Add((cur.ToDegrees().Lon, cur.ToDegrees().Lat));
                        }
                        else
                        {
                            temp.Add((-180.0, cutLat * 180.0 / Math.PI));
                            tracks[node.Value].Add(temp);
                            temp = new List<(double, double)>();
                            temp.Add((180.0, cutLat * 180.0 / Math.PI));
                            temp.Add((cur.ToDegrees().Lon, cur.ToDegrees().Lat));
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

        private static double linearInterpDiscontLat(Geo2D pp1, Geo2D pp2)
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

        public Satellite Satellite => _satellite;

        public IDictionary<int, IList<IList<(double lon, double lat)>>> GroundTracks => _tracks;
    }
}
