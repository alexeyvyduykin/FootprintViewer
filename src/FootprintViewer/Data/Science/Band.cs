using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FootprintViewer.Data.Science
{
    public class Band
    {
        public Band(Orbit orbit, double gam1DEG, double gam2DEG, BandMode mode)
        {
            Orbit = orbit;

            TrackPointDirection[] dir = new TrackPointDirection[2];
            switch (mode)
            {
                case BandMode.Middle:
                    dir[0] = TrackPointDirection.Left;
                    dir[1] = TrackPointDirection.Right;
                    break;
                case BandMode.Left:
                    dir[0] = dir[1] = TrackPointDirection.Left;
                    break;
                case BandMode.Right:
                    dir[0] = dir[1] = TrackPointDirection.Right;
                    break;
            }

            FactorShiftTrack factor = new FactorShiftTrack(orbit, gam1DEG, gam2DEG, mode);

            NearLine = new FactorTrack(new CustomTrack(orbit, gam1DEG, dir[0]), factor);
            FarLine = new FactorTrack(new CustomTrack(orbit, gam2DEG, dir[1]), factor);
        }

        public Band(Orbit orbit, double verticalHalfAngleDEG, double rollAngleDEG)
        {
            Orbit = orbit;

            BandMode mode;

            if (rollAngleDEG == 0)
            {
                mode = BandMode.Middle;
            }
            else if (rollAngleDEG > 0.0)
            {
                mode = BandMode.Left;
            }
            else
            {
                mode = BandMode.Right;
            }

            TrackPointDirection[] dir = new TrackPointDirection[2];
            switch (mode)
            {
                case BandMode.Middle:
                    dir[0] = TrackPointDirection.Left;
                    dir[1] = TrackPointDirection.Right;
                    break;
                case BandMode.Left:
                    dir[0] = dir[1] = TrackPointDirection.Left;
                    break;
                case BandMode.Right:
                    dir[0] = dir[1] = TrackPointDirection.Right;
                    break;
            }

            double gam1DEG = Math.Abs(rollAngleDEG) - verticalHalfAngleDEG;
            double gam2DEG = Math.Abs(rollAngleDEG) + verticalHalfAngleDEG;

            FactorShiftTrack factor = new FactorShiftTrack(orbit, gam1DEG, gam2DEG, mode);

            NearLine = new FactorTrack(new CustomTrack(orbit, gam1DEG, dir[0]), factor);
            FarLine = new FactorTrack(new CustomTrack(orbit, gam2DEG, dir[1]), factor);
        }

        public bool IsCoverPolis(double latRAD, ref double timeFromANToPolis)
        {
            double angleToPolis1 = 0.0, angleToPolis2 = 0.0;
            if (NearLine.PolisMod(latRAD, ref angleToPolis1) == true &&
                FarLine.PolisMod(latRAD, ref angleToPolis2) == true)
            {
                if (ScienceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
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
                if (ScienceMath.InRange(Math.PI / 2.0, angleToPolis1, angleToPolis2))
                {
                    return true;
                }
            }
            return false;
        }

        public Orbit Orbit { get; }

        public IList<Geo2D> GetNearGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
        {
            CustomTrack track1 = new CustomTrack(Orbit, NearLine.Alpha1 * ScienceMath.RadiansToDegrees, NearLine.Direction);
            return GetGroundTrack(track1, satellite, node, converter);
        }

        public IList<Geo2D> GetFarGroundTrack(PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
        {
            CustomTrack track2 = new CustomTrack(Orbit, FarLine.Alpha1 * ScienceMath.RadiansToDegrees, FarLine.Direction);
            return GetGroundTrack(track2, satellite, node, converter);
        }

        private IList<Geo2D> GetGroundTrack(CustomTrack track, PRDCTSatellite satellite, int node, Func<Geo2D, Geo2D>? converter = null)
        {
            converter = converter ?? ScienceConverters.From0To360;

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
            Band b1 = new Band(satellite.Orbit, -Math.Abs(sensor.VerticalHalfAngleDEG), sensor.RollAngleDEG);
            Band b2 = new Band(satellite.Orbit, Math.Abs(sensor.VerticalHalfAngleDEG), sensor.RollAngleDEG);

            var near1 = b1.GetNearGroundTrack(satellite, node).ToList();
            var far1 = b1.GetFarGroundTrack(satellite, node).ToList();

            near1.ForEach(s => { Geo2D.OffsetLeftLon(s); });
            far1.ForEach(s => { Geo2D.OffsetLeftLon(s); });

            var near2 = b2.GetNearGroundTrack(satellite, node).ToList();
            var far2 = b2.GetFarGroundTrack(satellite, node).ToList();

            near2.ForEach(s => { Geo2D.OffsetLeftLon(s); });
            far2.ForEach(s => { Geo2D.OffsetLeftLon(s); });

            var temp = System.Globalization.CultureInfo.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            using (StreamWriter writer = new StreamWriter(path, false))
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

            System.Threading.Thread.CurrentThread.CurrentCulture = temp;
        }

        //public List<List<Geo2D>> To2D(PRDCTSatellite satellite, int node, bool extrude = false, bool clockwise = true)
        //{
        //    var near = GetNearGroundTrack(satellite, node).ToList();
        //    var far = GetFarGroundTrack(satellite, node).ToList();

        //    near.ForEach(s => { Geo2D.OffsetLeftLon(s); });
        //    far.ForEach(s => { Geo2D.OffsetLeftLon(s); });


        //    BandCore2D engine2D = new BandCore2D(near, far, IsCoverPolis);

        //    engine2D.ExtrudeMode = extrude;
        //    engine2D.CreateShapes(clockwise, out List<List<Geo2D>> shapes);

        //    return shapes;
        //}

        public FactorTrack NearLine { get; private set; }
        public FactorTrack FarLine { get; private set; }
    }
}
