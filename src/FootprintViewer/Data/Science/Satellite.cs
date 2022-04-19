using System;
using System.Collections.Generic;

namespace FootprintViewer.Data.Science
{
    public class PRDCTSatellite
    {
        public PRDCTSatellite(Orbit orbit, DateTime startTime, DateTime stopTime, double trueAnomaly)
        {
            Orbit = orbit;
            StartTime = startTime;
            StopTime = stopTime;
            TrueAnomaly = trueAnomaly;
        }

        public PRDCTSatellite(Orbit orbit, DateTime startTime, DateTime stopTime) : this(orbit, startTime, stopTime, 0.0) { }

        public PRDCTSatellite(Orbit orbit, int days) : this(orbit, orbit.Epoch, orbit.Epoch.AddDays(days), 0.0) { }

        public PRDCTSatellite(Orbit orbit, int days, double trueAnomaly) : this(orbit, orbit.Epoch, orbit.Epoch.AddDays(days), trueAnomaly) { }

        public Orbit Orbit { get; }

        public double TrueAnomaly { get; }

        public DateTime StartTime { get; }

        public DateTime StopTime { get; }

        public double TrueTimePastAN
        {
            get
            {
                double u = TrueAnomaly;
                double n = Math.Sqrt(Constants.GM) * Math.Pow(Orbit.SemimajorAxis, -3.0 / 2.0);
                double e1 = 2.0 * Math.Atan2(Math.Sqrt((1.0 - Orbit.Eccentricity) / (1.0 + Orbit.Eccentricity)) * Math.Sin(u / 2.0), Math.Cos(u / 2.0));
                if (e1 < 0)
                {
                    e1 += 2.0 * Math.PI;
                }

                double e2 = e1 - Orbit.Eccentricity * Math.Sin(e1);
                return e2 / n;
            }
        }

        public List<Node> Nodes()
        {
            var nodes = new List<Node>();
            //double julianDate_ = StartTime.ToOADate() + 2415018.5;
            //double TimeBegin = StartTime.TimeOfDay.TotalSeconds;
            double TimeDuration = (StopTime - StartTime).TotalSeconds;
            double TimeEnd = TimeDuration + TrueTimePastAN;

            int numNodes = (int)Math.Ceiling((TimeDuration + TrueTimePastAN) / Orbit.Period);

            double timePastAN = TrueTimePastAN;

            for (int i = 0; i < numNodes; i++)
            {
                var node = new Node();

                var tq = new List<Tuple<double, double, int>>();

                for (int j = 1; j <= 4; j++)
                {
                    double q = Orbit.Quarts[j] + i * Orbit.Period;
                    if (timePastAN < q)
                    {
                        if (TimeEnd < q)
                        {
                            tq.Add(Tuple.Create(timePastAN, TimeEnd, j));
                            break;
                        }

                        tq.Add(Tuple.Create(timePastAN, q, j));
                        timePastAN = q;
                    }
                }

                #region Full Realization

                //   double q1 = Orbit.Quarts[1] + i * Orbit.Period;
                //   double q2 = Orbit.Quarts[2] + i * Orbit.Period;
                //   double q3 = Orbit.Quarts[3] + i * Orbit.Period;
                //   double q4 = Orbit.Quarts[4] + i * Orbit.Period;

                //if (timePastAN < q1)
                //{
                //    if (TimeEnd < q1)
                //    {
                //        tq.Add(Tuple.Create(timePastAN, TimeEnd, 1));
                //        goto markl;
                //    }

                //    tq.Add(Tuple.Create(timePastAN, q1, 1));
                //    timePastAN = q1;
                //}

                //if (timePastAN < q2)
                //{
                //    if (TimeEnd < q2)
                //    {
                //        tq.Add(Tuple.Create(timePastAN, TimeEnd, 2));
                //        goto markl;
                //    }

                //    tq.Add(Tuple.Create(timePastAN, q2, 2));
                //    timePastAN = q2;
                //}

                //if (timePastAN < q3)
                //{
                //    if (TimeEnd < q3)
                //    {
                //        tq.Add(Tuple.Create(timePastAN, TimeEnd, 3));
                //        goto markl;
                //    }

                //    tq.Add(Tuple.Create(timePastAN, q3, 3));
                //    timePastAN = q3;
                //}

                //if (timePastAN < q4)
                //{
                //    if (TimeEnd < q4)
                //    {
                //        tq.Add(Tuple.Create(timePastAN, TimeEnd, 4));
                //        goto markl;
                //    }

                //    tq.Add(Tuple.Create(timePastAN, q4, 4));
                //    timePastAN = q4;
                //}

                //markl:

                #endregion

                foreach (var item in tq)
                {
                    node.Quarts.Add(new NodeQuarter
                    {
                        TimeBegin = item.Item1 - TrueTimePastAN,
                        TimeEnd = item.Item2 - TrueTimePastAN,
                        Quart = item.Item3
                    });
                }

                node.Value = i + 1;
                nodes.Add(node);
            }

            //Console.WriteLine("Nodes: TrueTimePastAN = {0}, Period = {1}, numNodes = {2}", TrueTimePastAN, Orbit.Period, numNodes);
            return nodes;
        }

        public List<Geo2D> GetGroundTrack(int node)
        {
            var points = new List<Geo2D>();

            var track = new Track(Orbit);

            var nodes = Nodes();

            if (nodes.Count <= node)
            {
                return points;
            }

            for (int q = 0; q < nodes[node].Quarts.Count; q++)
            {
                for (double t = nodes[node].Quarts[q].TimeBegin; t <= nodes[node].Quarts[q].TimeEnd; t += 5.0)
                {
                    var point = track.ContinuousTrack(node, t, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    double lon = point.Lon;
                    while (lon > 2.0 * Math.PI)
                    {
                        lon -= 2.0 * Math.PI;
                    }

                    while (lon < 0.0)
                    {
                        lon += 2.0 * Math.PI;
                    }

                    points.Add(new Geo2D(lon, point.Lat));
                }
            }

            return points;
        }

        public List<Geo2D> GetGroundTrack(int node, int parts)
        {
            var points = new List<Geo2D>();

            var track = new Track(Orbit);

            var nodes = Nodes();

            if (nodes.Count <= node)
            {
                return points;
            }

            var ttt1 = nodes[node].Quarts[0].TimeBegin;
            var ttt2 = nodes[node].Quarts[^1].TimeEnd;

            double step = (ttt2 - ttt1) / parts;

            double last = 0;

            for (int q = 0; q < nodes[node].Quarts.Count; q++)
            {
                double t1 = nodes[node].Quarts[q].TimeBegin;
                double t2 = nodes[node].Quarts[q].TimeEnd;

                t1 += last;

                for (double t = t1; t <= t2; t += step)
                {
                    var point = track.ContinuousTrack(node, t, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    double lon = point.Lon;
                    while (lon > 2.0 * Math.PI)
                    {
                        lon -= 2.0 * Math.PI;
                    }

                    while (lon < 0.0)
                    {
                        lon += 2.0 * Math.PI;
                    }

                    points.Add(new Geo2D(lon, point.Lat));

                    last = t;
                }

                last = step - (t2 - last);

                if (q == nodes[node].Quarts.Count - 1 && last != 0.0)
                {
                    var point = track.ContinuousTrack(node, t2, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    double lon = point.Lon;
                    while (lon > 2.0 * Math.PI)
                    {
                        lon -= 2.0 * Math.PI;
                    }

                    while (lon < 0.0)
                    {
                        lon += 2.0 * Math.PI;
                    }

                    points.Add(new Geo2D(lon, point.Lat));
                }
            }

            return points;
        }

        /// <summary>
        /// lon(rad) => (-PI; +PI)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public List<Geo2D> GetGroundTrackDynStep1(int node, double seconds)
        {
            var points = new List<Geo2D>();

            var track = new Track(Orbit);

            var nodes = Nodes();

            if (nodes.Count <= node)
            {
                return points;
            }

            var ttt1 = nodes[node].Quarts[0].TimeBegin;
            var ttt2 = nodes[node].Quarts[^1].TimeEnd;

            double angle = seconds * 2.0 * Math.PI / (ttt2 - ttt1);
            double sec = 1.0 / Math.Cos(angle);

            double step = seconds / sec;

            double last = 0;

            for (int q = 0; q < nodes[node].Quarts.Count; q++)
            {
                double t1 = nodes[node].Quarts[q].TimeBegin;
                double t2 = nodes[node].Quarts[q].TimeEnd;

                t1 += last;

                for (double t = t1; t <= t2; t += step)
                {
                    var point = track.ContinuousTrack(node, t, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    double lon = point.Lon;
                    while (lon > Math.PI)
                    {
                        lon -= 2.0 * Math.PI;
                    }

                    while (lon < -Math.PI)
                    {
                        lon += 2.0 * Math.PI;
                    }

                    points.Add(new Geo2D(lon, point.Lat));

                    last = t;

                    sec = 1.0 / Math.Cos(point.Lat);
                    step = seconds / sec;
                }

                last = step - (t2 - last);

                if (q == nodes[node].Quarts.Count - 1 && last != 0.0)
                {
                    var point = track.ContinuousTrack(node, t2, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    double lon = point.Lon;
                    while (lon > Math.PI)
                    {
                        lon -= 2.0 * Math.PI;
                    }

                    while (lon < -Math.PI)
                    {
                        lon += 2.0 * Math.PI;
                    }

                    points.Add(new Geo2D(lon, point.Lat));
                }
            }

            return points;
        }

        public List<Geo2D> GetGroundTrackDynStep(int node, double seconds, Func<Geo2D, Geo2D>? converter = null)
        {
            converter ??= ScienceConverters.From0To360;

            var points = new List<Geo2D>();

            var track = new Track(Orbit);

            var nodes = Nodes();

            if (nodes.Count <= node)
            {
                return points;
            }

            var ttt1 = nodes[node].Quarts[0].TimeBegin;
            var ttt2 = nodes[node].Quarts[^1].TimeEnd;

            double angle = seconds * 2.0 * Math.PI / (ttt2 - ttt1);
            double sec = 1.0 / Math.Cos(angle);

            double step = seconds / sec;

            double last = 0;

            for (int q = 0; q < nodes[node].Quarts.Count; q++)
            {
                double t1 = nodes[node].Quarts[q].TimeBegin;
                double t2 = nodes[node].Quarts[q].TimeEnd;

                t1 += last;

                for (double t = t1; t <= t2; t += step)
                {
                    var point = track.ContinuousTrack(node, t, TrueTimePastAN, nodes[node].Quarts[q].Quart);

                    points.Add(converter.Invoke(point));

                    last = t;

                    sec = 1.0 / Math.Cos(point.Lat);
                    step = seconds / sec;
                }

                last = step - (t2 - last);

                if (q == nodes[node].Quarts.Count - 1 && last != 0.0)
                {
                    var point = track.ContinuousTrack(node, t2, TrueTimePastAN, nodes[node].Quarts[q].Quart);
                    points.Add(converter.Invoke(point));
                }
            }

            return points;
        }
    }
}
