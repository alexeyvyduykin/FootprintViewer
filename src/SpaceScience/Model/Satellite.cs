namespace SpaceScience.Model;

public class PRDCTSatellite
{
    internal PRDCTSatellite(Orbit orbit, DateTime startTime, DateTime stopTime, double trueAnomaly)
    {
        Orbit = orbit;
        StartTime = startTime;
        StopTime = stopTime;
        TrueAnomaly = trueAnomaly;
    }

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
}
