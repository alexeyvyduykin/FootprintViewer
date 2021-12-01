using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Data.Science
{

    public class Orbit
    {
        public Orbit(double a, double ecc, double incl, double argOfPer, double lonAN, double om, double period, DateTime epoch)
        {
            this.SemimajorAxis = a;
            this.Eccentricity = ecc;
            this.Inclination = incl;
            this.ArgumentOfPerigee = argOfPer;
            this.LonAscnNode = lonAN;
            this.RAAN = om;
            this.Period = period;
            this.Epoch = epoch;

            pTemp = a * (1 - ecc * ecc);
            nTemp = Math.Sqrt(Constants.GM / a) / a;
            timeHalfPi = TimeHalfPi();
        }

        public Orbit(Orbit orbit) : this(orbit.SemimajorAxis, orbit.Eccentricity, orbit.Inclination, orbit.ArgumentOfPerigee, orbit.LonAscnNode, orbit.RAAN, orbit.Period, orbit.Epoch) { }

        public double Anomalia(double tNorm, double tPastAN = 0.0)
        {
            double M, v, e1, e2;
            M = nTemp * (tNorm + tPastAN);

            e1 = M;
            e2 = M + Eccentricity * Math.Sin(e1);
            while (Math.Abs(e1 - e2) > 0.001)
            {
                e1 = e2;
                e2 = M + Eccentricity * Math.Sin(e1);
            }
            v = Math.Atan2(Math.Sin(e2) * Math.Sqrt(1 - Eccentricity * Eccentricity), Math.Cos(e2) - Eccentricity);
            v = ScienceMath.WrapAngle(v);
            return v;
        }

        public double Radius(double tnorm)
        {
            if (Eccentricity == 0)
                return SemimajorAxis;
            double u = Anomalia(tnorm) + ArgumentOfPerigee;
            return pTemp / (1 + Eccentricity * Math.Cos(u));
        }

        public double Semiaxis(double u)
        {
            return pTemp / (1 + Eccentricity * Math.Cos(u));
        }

        public double TimeHalfPi()
        {
            double r = Math.PI / 4.0;
            double e1 = 2.0 * Math.Atan2(Math.Sqrt((1.0 - Eccentricity) / (1.0 + Eccentricity)) * Math.Sin(r), Math.Cos(r));
            if (e1 < 0) e1 += 2.0 * Math.PI;
            double e2 = e1 - Eccentricity * Math.Sin(e1);
            return e2 / nTemp;
        }

        public double InclinationNormal
        {
            get
            {
                if (Inclination >= 0.0 && Inclination <= Math.PI / 2.0)
                    return Inclination;
                return Math.PI - Inclination;
            }
        }

        public double Quart0 { get { return 0.0; } }
        public double Quart1 { get { return timeHalfPi; } }
        public double Quart2 { get { return Period / 2.0; } }
        public double Quart3 { get { return Period - timeHalfPi; } }
        public double Quart4 { get { return Period; } }

        public double[] Quarts { get { return new double[5] { 0.0, timeHalfPi, Period / 2.0, Period - timeHalfPi, Period }; } }

        public double SemimajorAxis { get; }
        public double Eccentricity { get; }
        public double Inclination { get; }
        public double ArgumentOfPerigee { get; }
        public double LonAscnNode { get; }
        public double RAAN { get; }
        public double Period { get; }
        public DateTime Epoch { get; }

        private double nTemp, pTemp;
        private double timeHalfPi;
    }


}
