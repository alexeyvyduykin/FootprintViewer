using System;

namespace FootprintViewer.Data.Science
{
    public class Orbit
    {
        private readonly double _nTemp;
        private readonly double _pTemp;
        private readonly double _timeHalfPi;

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

            _pTemp = a * (1 - ecc * ecc);
            _nTemp = Math.Sqrt(Constants.GM / a) / a;
            _timeHalfPi = TimeHalfPi();
        }

        public Orbit(Orbit orbit) : this(orbit.SemimajorAxis, orbit.Eccentricity, orbit.Inclination, orbit.ArgumentOfPerigee, orbit.LonAscnNode, orbit.RAAN, orbit.Period, orbit.Epoch) { }

        public static double Quart0 => 0.0;

        public double Quart1 => _timeHalfPi;

        public double Quart2 => Period / 2.0;

        public double Quart3 => Period - _timeHalfPi;

        public double Quart4 => Period;

        public double[] Quarts => new[] { 0.0, _timeHalfPi, Period / 2.0, Period - _timeHalfPi, Period };

        public double SemimajorAxis { get; }

        public double Eccentricity { get; }

        public double Inclination { get; }

        public double ArgumentOfPerigee { get; }

        public double LonAscnNode { get; }

        public double RAAN { get; }

        public double Period { get; }

        public DateTime Epoch { get; }

        public double Anomalia(double tNorm, double tPastAN = 0.0)
        {
            double M, v, e1, e2;
            M = _nTemp * (tNorm + tPastAN);

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
            return _pTemp / (1 + Eccentricity * Math.Cos(u));
        }

        public double Semiaxis(double u)
        {
            return _pTemp / (1 + Eccentricity * Math.Cos(u));
        }

        public double TimeHalfPi()
        {
            double r = Math.PI / 4.0;
            double e1 = 2.0 * Math.Atan2(Math.Sqrt((1.0 - Eccentricity) / (1.0 + Eccentricity)) * Math.Sin(r), Math.Cos(r));
            if (e1 < 0)
                e1 += 2.0 * Math.PI;
            double e2 = e1 - Eccentricity * Math.Sin(e1);
            return e2 / _nTemp;
        }

        public double InclinationNormal => (Inclination >= 0.0 && Inclination <= Math.PI / 2.0) ? Inclination : Math.PI - Inclination;
    }
}
