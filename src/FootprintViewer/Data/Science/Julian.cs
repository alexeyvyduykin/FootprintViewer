using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Science
{
    public class Julian
    {
        private const double EPOCH_JAN0_12H_1900 = 2415020.0; // Dec 31.5 1899 = Dec 31 1899 12h UTC
        private const double EPOCH_JAN1_00H_1900 = 2415020.5; // Jan  1.0 1900 = Jan  1 1900 00h UTC
        private const double EPOCH_JAN1_12H_1900 = 2415021.0; // Jan  1.5 1900 = Jan  1 1900 12h UTC
        private const double EPOCH_JAN1_12H_2000 = 2451545.0; // Jan  1.5 2000 = Jan  1 2000 12h UTC
        private const double EPOCH_J2000 = 2451544.99926;     // Jan  1 2000 11h 58m 55.816s UTC

        public Julian(DateTime utc)
        {
            double day = utc.DayOfYear + (utc.Hour + ((utc.Minute + ((utc.Second + (utc.Millisecond / 1000.0)) / 60.0)) / 60.0)) / 24.0;
            Initialize(utc.Year, day);
        }

        private void Initialize(int year, double day)
        {
            // Arbitrary years used for error checking
            if (year < 1900 || year > 2100)
            {
                throw new ArgumentOutOfRangeException("year");
            }

            // The last day of a leap year is day 366
            if (day < 1.0 || day >= 367.0)
            {
                throw new ArgumentOutOfRangeException("day");
            }

            this.Year = year;
            this.Day = day;

            year--;

            int A = (year / 100);
            int B = 2 - A + (A / 4);

            double NewYears = (int)(365.25 * year) + (int)(30.6001 * 14) + 1720994.5 + B;

            this.Date = NewYears + day;
        }

        // Calculate Greenwich Mean Sidereal Time for the Julian date
        //public double ToGmst()
        //{
        //    double UT = (Date + 0.5) % 1.0;
        //    //double TU = (FromJan1_12h_2000() - UT) / 36525.0;
        //    double TU = (FromJan1_12h_2000() - UT) / 36525.0;


        //    double GMST = 24110.54841 + TU * (8640184.812866 + TU * (0.093104 - TU * 6.2e-06));

        //    GMST = (GMST + Globals.SecPerDay * Globals.OmegaE * UT) % Globals.SecPerDay;

        //    if (GMST < 0.0)
        //    {
        //        GMST += Globals.SecPerDay;  // "wrap" negative modulo value
        //    }

        //    return (2.0 * Math.PI * (GMST / Globals.SecPerDay));
        //}

        public double ToGmst()
        {
            // greenwich mean sidereal time
            return Meeus.gast2(Date, 0.0, 0) * 360.0 / 24.0;
        }

        public double ToGast()
        {
            // greenwich apparent sidereal time
            return Meeus.gast2(Date, 0.0, 1) * 360.0 / 24.0;
        }

        public double Date { get; private set; } // Julian date

        public double FromJan0_12h_1900() { return Date - EPOCH_JAN0_12H_1900; }
        public double FromJan1_00h_1900() { return Date - EPOCH_JAN1_00H_1900; }
        public double FromJan1_12h_1900() { return Date - EPOCH_JAN1_12H_1900; }
        public double FromJan1_12h_2000() { return Date - EPOCH_JAN1_12H_2000; }
        public double FromJ2000() { return Date - EPOCH_J2000; }

        private int Year { get; set; }    // Year including century
        private double Day { get; set; }  // Day of year, 1.0 = Jan 1 00h
    }

    internal static class Meeus
    {
        private static List<List<double>> xnod;
        private static List<double> sdata;

        public static double julian(double day, int year)
        {
            // Julian date

            // Arbitrary years used for error checking
            if (year < 1900 || year > 2100)
            {
                throw new ArgumentOutOfRangeException("year");
            }

            // The last day of a leap year is day 366
            if (day < 1.0 || day >= 367.0)
            {
                throw new ArgumentOutOfRangeException("day");
            }

            year--;

            int A = year / 100;
            int B = 2 - A + (A / 4);

            double NewYears = (int)(365.25 * year) + (int)(30.6001 * 14) + 1720994.5 + B;
            double jdate = NewYears + day;
            return jdate;
        }

        public static double gast2(double tjdh, double tjdl, int k)
        {
            // this function computes the greenwich sidereal time
            // (either mean or apparent) at julian date tjdh + tjdl

            // nutation parameters from function nod

            // input

            // tjdh = julian date, high - order part

            // tjdl = julian date, low - order part

            // julian date may be split at any point, but for
            // highest precision, set tjdh to be the integral part of
            // the julian date, and set tjdl to be the fractional part

            // k = time selection code
            // set k = 0 for greenwich mean sidereal time
            // set k = 1 for greenwich apparent sidereal time

            // output

            // gst = greenwich(mean or apparent) sidereal time in hours

            double seccon = 206264.8062470964;

            double t0 = 2451545.0;

            double tjd = tjdh + tjdl;
            double th = (tjdh - t0) / 36525;
            double tl = tjdl / 36525.0;
            double t = th + tl;
            double t2 = t * t;
            double t3 = t2 * t;

            // for apparent sidereal time, obtain equation of the equinoxes

            double eqeq = 0.0;

            if (k == 1)
            {

                // obtain nutation parameters in seconds of arc

                var res = nod(tjd, 1);
                double psi = res.Item1;
                double eps = res.Item2;

                // compute mean obliquity of the ecliptic in seconds of arc

                double obm = 84381.4480 - 46.8150 * t - 0.00059 * t2 + 0.001813 * t3;

                // compute true obliquity of the ecliptic in seconds of arc

                double obt = obm + eps;

                // compute equation of the equinoxes in seconds of time

                eqeq = psi / 15.0 * Math.Cos(obt / seccon);

            }

            double st = eqeq - 6.2e-6 * t3 + 0.093104 * t2 + 67310.54841

                 + 8640184.812866 * tl + 3155760000 * tl

                 + 8640184.812866 * th + 3155760000 * th;

            double gst = (st / 3600.0) % 24;

            if (gst < 0.0)
                gst = gst + 24;

            return gst;
        }

        public static Tuple<double, double> nod(double jdate, int inutate)
        {
            // 79 - 106. 1980 iau theory of nutation.

            // jdate = tdb julian date (in)
            // dpsi = nutation in longitude in arcseconds(out)
            // deps = nutation in obliquity in arcseconds(out)

            // NOTE: requires first time initialization via inutate flag

            // Orbital Mechanics with Matlab

            if (inutate == 1)
            {
                // read coefficient data file

                //     xnod = csvread("nut80.csv");

                #region xnod

                xnod = new List<List<double>> {
 new List<double> { 0.0,  0.0,  0.0,  0.0,  1.0, -171996.0, -174.2,  92025.0,  8.9 },
  new List<double> { 0.0,  0.0,  2.0, -2.0,  2.0,  -13187.0,   -1.6,   5736.0, -3.1 },
  new List<double> { 0.0,  0.0,  2.0,  0.0,  2.0,   -2274.0,   -0.2,    977.0, -0.5 },
  new List<double> { 0.0,  0.0,  0.0,  0.0,  2.0,    2062.0,    0.2,   -895.0,  0.5 },
  new List<double> { 0.0,  1.0,  0.0,  0.0,  0.0,    1426.0,   -3.4,     54.0, -0.1 },
  new List<double> { 1.0,  0.0,  0.0,  0.0,  0.0,     712.0,    0.1,     -7.0,  0.0 },
  new List<double> { 0.0,  1.0,  2.0, -2.0,  2.0,    -517.0,    1.2,    224.0, -0.6 },
  new List<double> { 0.0,  0.0,  2.0,  0.0,  1.0,    -386.0,   -0.4,    200.0,  0.0 },
  new List<double> { 1.0,  0.0,  2.0,  0.0,  2.0,    -301.0,    0.0,    129.0, -0.1 },
  new List<double> { 0.0, -1.0,  2.0, -2.0,  2.0,     217.0,   -0.5,    -95.0,  0.3 },
  new List<double> { 1.0,  0.0,  0.0, -2.0,  0.0,    -158.0,    0.0,     -1.0,  0.0 },
  new List<double> { 0.0,  0.0,  2.0, -2.0,  1.0,     129.0,    0.1,    -70.0,  0.0 },
new List<double> { -1.0,  0.0,  2.0,  0.0,  2.0,     123.0,    0.0,    -53.0,  0.0},
 new List<double> { 1.0,  0.0,  0.0,  0.0,  1.0,      63.0,    0.1,    -33.0,  0.0},
 new List<double> { 0.0,  0.0,  0.0,  2.0,  0.0,      63.0,    0.0,     -2.0,  0.0},
new List<double> { -1.0,  0.0,  2.0,  2.0,  2.0,     -59.0,    0.0,     26.0,  0.0},
new List<double> { -1.0,  0.0,  0.0,  0.0,  1.0,     -58.0,   -0.1,     32.0,  0.0},
 new List<double> { 1.0,  0.0,  2.0,  0.0,  1.0,     -51.0,    0.0,     27.0,  0.0},
 new List<double> { 2.0,  0.0,  0.0, -2.0,  0.0,      48.0,    0.0,      1.0,  0.0},
new List<double> { -2.0,  0.0,  2.0,  0.0,  1.0,      46.0,    0.0,    -24.0,  0.0},
 new List<double> { 0.0,  0.0,  2.0,  2.0,  2.0,     -38.0,    0.0,     16.0,  0.0},
 new List<double> { 2.0,  0.0,  2.0,  0.0,  2.0,     -31.0,    0.0,     13.0,  0.0},
 new List<double> { 2.0,  0.0,  0.0,  0.0,  0.0,      29.0,    0.0,     -1.0,  0.0},
 new List<double> { 1.0,  0.0,  2.0, -2.0,  2.0,      29.0,    0.0,    -12.0,  0.0},
 new List<double> { 0.0,  0.0,  2.0,  0.0,  0.0,      26.0,    0.0,     -1.0,  0.0},
 new List<double> { 0.0,  0.0,  2.0, -2.0,  0.0,     -22.0,    0.0,      0.0,  0.0},
new List<double> { -1.0,  0.0,  2.0,  0.0,  1.0,      21.0,    0.0,    -10.0,  0.0},
 new List<double> { 0.0,  2.0,  0.0,  0.0,  0.0,      17.0,   -0.1,      0.0,  0.0},
 new List<double> { 0.0,  2.0,  2.0, -2.0,  2.0,     -16.0,    0.1,      7.0,  0.0},
 new List<double> { -1.0,  0.0,  0.0,  2.0,  1.0,      16.0,    0.00,     -8.0,  0.00},
 new List<double> { 0.0,  1.0,  0.0,  0.0,  1.0,     -15.0,    0.00,      9.0,  0.00},
 new List<double> { 1.0,  0.0,  0.0, -2.0,  1.0,     -13.0,    0.00,      7.0,  0.00},
 new List<double> { 0.0, -1.0,  0.0,  0.0,  1.0,     -12.0,    0.00,      6.0,  0.00},
 new List<double> { 2.0,  0.0, -2.0,  0.0,  0.0,      11.0,    0.00,      0.0,  0.00},
new List<double> { -1.0,  0.0,  2.0,  2.0,  1.0,     -10.0,    0.00,      5.0,  0.00},
 new List<double> { 1.0,  0.0,  2.0,  2.0,  2.0,      -8.0,    0.00,      3.0,  0.00},
 new List<double> { 0.0, -1.0,  2.0,  0.0,  2.0,      -7.0,    0.00,      3.0,  0.00},
 new List<double> { 0.0,  0.0,  2.0,  2.0,  1.0,      -7.0,    0.00,      3.0,  0.00},
 new List<double> { 1.0,  1.0,  0.0, -2.0,  0.0,      -7.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  1.0,  2.0,  0.0,  2.0,       7.0,    0.00,     -3.0,  0.00},
new List<double> { -2.0,  0.0,  0.0,  2.0,  1.0,      -6.0,    0.00,      3.0,  0.00},
 new List<double> { 0.0,  0.0,  0.0,  2.0,  1.0,      -6.0,    0.00,      3.0,  0.00},
 new List<double> { 2.0,  0.0,  2.0, -2.0,  2.0,       6.0,    0.00,     -3.0,  0.00},
 new List<double> { 1.0,  0.0,  0.0,  2.0,  0.0,       6.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0,  0.0,  2.0, -2.0,  1.0,       6.0,    0.00,     -3.0,  0.00},
 new List<double> { 0.0,  0.0,  0.0, -2.0,  1.0,      -5.0,    0.00,      3.0,  0.00},
 new List<double> { 0.0, -1.0,  2.0, -2.0,  1.0,      -5.0,    0.00,      3.0,  0.00},
 new List<double> { 2.0,  0.0,  2.0,  0.0,  1.0,      -5.0,    0.00,      3.0,  0.00},
 new List<double> { 1.0, -1.0,  0.0,  0.0,  0.0,       5.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0,  0.0,  0.0, -1.0,  0.0,      -4.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  0.0,  0.0,  1.0,  0.0,      -4.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  1.0,  0.0, -2.0,  0.0,      -4.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0,  0.0, -2.0,  0.0,  0.0,       4.0,    0.00,      0.0,  0.00},
 new List<double> { 2.0,  0.0,  0.0, -2.0,  1.0,       4.0,    0.00,     -2.0,  0.00},
 new List<double> { 0.0,  1.0,  2.0, -2.0,  1.0,       4.0,    0.00,     -2.0,  0.00},
 new List<double> { 1.0,  1.0,  0.0,  0.0,  0.0,      -3.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0, -1.0,  0.0, -1.0,  0.0,      -3.0,    0.00,      0.0,  0.00},
new List<double> { -1.0, -1.0,  2.0,  2.0,  2.0,      -3.0,    0.00,      1.0,  0.00},
 new List<double> { 0.0, -1.0,  2.0,  2.0,  2.0,      -3.0,    0.00,      1.0,  0.00},
 new List<double> { 1.0, -1.0,  2.0,  0.0,  2.0,      -3.0,    0.00,      1.0,  0.00},
 new List<double> { 3.0,  0.0,  2.0,  0.0,  2.0,      -3.0,    0.00,      1.0,  0.00},
new List<double> { -2.0,  0.0,  2.0,  0.0,  2.0,      -3.0,    0.00,      1.0,  0.00},
 new List<double> { 1.0,  0.0,  2.0,  0.0,  0.0,       3.0,    0.00,      0.0,  0.00},
new List<double> { -1.0,  0.0,  2.0,  4.0,  2.0,      -2.0,    0.00,      1.0,  0.00},
 new List<double> { 1.0,  0.0,  0.0,  0.0,  2.0,      -2.0,    0.00,      1.0,  0.00},
new List<double> { -1.0,  0.0,  2.0, -2.0,  1.0,      -2.0,    0.00,      1.0,  0.00},
 new List<double> { 0.0, -2.0,  2.0, -2.0,  1.0,      -2.0,    0.00,      1.0,  0.00},
new List<double> { -2.0,  0.0,  0.0,  0.0,  1.0,      -2.0,    0.00,      1.0,  0.00},
 new List<double> { 2.0,  0.0,  0.0,  0.0,  1.0,       2.0,    0.00,     -1.0,  0.00},
 new List<double> { 3.0,  0.0,  0.0,  0.0,  0.0,       2.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0,  1.0,  2.0,  0.0,  2.0,       2.0,    0.00,     -1.0,  0.00},
 new List<double> { 0.0,  0.0,  2.0,  1.0,  2.0,       2.0,    0.00,     -1.0,  0.00},
 new List<double> { 1.0,  0.0,  0.0,  2.0,  1.0,      -1.0,    0.00,      0.0,  0.00},
 new List<double> { 1.0,  0.0,  2.0,  2.0,  1.0,      -1.0,    0.00,      1.0,  0.00},
 new List<double> { 1.0,  1.0,  0.0, -2.0,  1.0,      -1.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  1.0,  0.0,  2.0,  0.0,      -1.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  1.0,  2.0, -2.0,  0.0,      -1.0,    0.00,      0.0,  0.00},
 new List<double> { 0.0,  1.0, -2.0,  2.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 1.0,  0.0, -2.0,  2.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 1.0,  0.0, -2.0, -2.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
new List<double> {  1.0,  0.0,  2.0, -2.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 1.0,  0.0,  0.0, -4.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 2.0,  0.0,  0.0, -4.0,  0.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  0.0,  2.0,  4.0,  2.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  0.0,  2.0, -1.0,  2.0,      -1.0,    0.00,      0.0,  0.00 },
new List<double> { -2.0,  0.0,  2.0,  4.0,  2.0,      -1.0,    0.00,      1.0,  0.00 },
 new List<double> { 2.0,  0.0,  2.0,  2.0,  2.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0, -1.0,  2.0,  0.0,  1.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  0.0, -2.0,  0.0,  1.0,      -1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  0.0,  4.0, -2.0,  2.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  1.0,  0.0,  0.0,  2.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 1.0,  1.0,  2.0, -2.0,  2.0,       1.0,    0.00,     -1.0,  0.00 },
 new List<double> { 3.0,  0.0,  2.0, -2.0,  2.0,       1.0,    0.00,      0.0,  0.00 },
new List<double> { -2.0,  0.0,  2.0,  2.0,  2.0,       1.0,    0.00,     -1.0,  0.00 },
new List<double> { -1.0,  0.0,  0.0,  0.0,  2.0,       1.0,    0.00,     -1.0,  0.00 },
 new List<double> { 0.0,  0.0, -2.0,  2.0,  1.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  1.0,  2.0,  0.0,  1.0,       1.0,    0.00,      0.0,  0.00 },
new List<double> { -1.0,  0.0,  4.0,  0.0,  2.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 2.0,  1.0,  0.0, -2.0,  0.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 2.0,  0.0,  0.0,  2.0,  0.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 2.0,  0.0,  2.0, -2.0,  1.0,       1.0,    0.00,     -1.0,  0.00 },
 new List<double> { 2.0,  0.0, -2.0,  0.0,  1.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 1.0, -1.0,  0.0, -2.0,  0.0,       1.0,    0.00,      0.0,  0.00 },
new List<double> { -1.0,  0.0,  0.0,  1.0,  1.0,       1.0,    0.00,      0.0,  0.00 },
new List<double> { -1.0, -1.0,  0.0,  2.0,  1.0,       1.0,    0.00,      0.0,  0.00 },
 new List<double> { 0.0,  1.0,  0.0,  1.0,  0.0,       1.0,    0.00,      0.0,  0.00 }

 };
                #endregion

                inutate = 0;
            }

            // time argument in julian centuries

            double tjcent = (jdate - 2451545.0) / 36525.0;

            // get fundamental arguments
            var res = funarg(tjcent);
            double l = res.Item1;
            double lp = res.Item2;
            double f = res.Item3;
            double d = res.Item4;
            double om = res.Item5;

            // sum nutation series terms, from smallest to largest

            double dpsi = 0.0;

            double deps = 0.0;

            for (int j = 0; j < 106; j++)
            {

                int i = 105 - j;


                // formation of multiples of arguments

                double arg = xnod[i][0] * l + xnod[i][1] * lp + xnod[i][2] * f + xnod[i][3] * d + xnod[i][4] * om;

                // evaluate nutation

                dpsi = (xnod[i][5] + xnod[i][6] * tjcent) * Math.Sin(arg) + dpsi;

                deps = (xnod[i][7] + xnod[i][8] * tjcent) * Math.Cos(arg) + deps;

            }

            dpsi = dpsi * 1.0e-4;

            deps = deps * 1.0e-4;

            return Tuple.Create(dpsi, deps);

        }

        public static Tuple<double, double, double> sun2(double jdate, int suncoef)
        {
            //    function[rasc, decl, rsun] = sun2(jdate)

            // precision ephemeris of the Sun

            // input

            // jdate = julian ephemeris date

            // output

            // rasc = right ascension of the Sun (radians)
            // (0 <= rasc <= 2 pi)
            // decl = declination of the Sun(radians)
            // (-pi / 2 <= decl <= pi / 2)
            // rsun = eci position vector of the Sun(km)

            // note

            // coordinates are inertial, geocentric,
            // equatorial and true - of - date




            // fundamental time argument

            double u = (jdate - 2451545) / 3652500;

            // compute nutation in longitude

            double a1 = 2.18 + u * (-3375.7 + u * 0.36);
            double a2 = 3.51 + u * (125666.39 + u * 0.1);

            double psi = 0.0000001 * (-834 * Math.Sin(a1) - 64 * Math.Sin(a2));

            // compute nutation in obliquity

            double deps = 0.0000001 * u * (-226938 + u * (-75 + u * (96926 + u * (-2491 - u * 12104))));

            double meps = 0.0000001 * (4090928 + 446 * Math.Cos(a1) + 28 * Math.Cos(a2));

            double eps = meps + deps;

            double seps = Math.Sin(eps);
            double ceps = Math.Cos(eps);

            double dl = 0;
            double dr = 0;

            // global suncoef sdata rlsun

            if (suncoef == 1)
            {
                #region sdata

                sdata = new List<double> {
                403406, 0, 4.721964, 1.621043,
                195207, -97597, 5.937458, 62830.348067,
                119433, -59715, 1.115589, 62830.821524,
                112392, -56188, 5.781616, 62829.634302,
                3891, -1556, 5.5474, 125660.5691,
                2819, -1126, 1.5120, 125660.9845,
                1721, -861, 4.1897, 62832.4766,
                0, 941, 1.163, 0.813,
                660, -264, 5.415, 125659.310,
                350, -163, 4.315, 57533.850,
                334, 0, 4.553, -33.931,
                314, 309, 5.198, 777137.715,
                268, -158, 5.989, 78604.191,
                242, 0, 2.911, 5.412,
                234, -54, 1.423, 39302.098,
                158, 0, 0.061, -34.861,
                132, -93, 2.317, 115067.698,
                129, -20, 3.193, 15774.337,
                114, 0, 2.828, 5296.670,
                99, -47, 0.52, 58849.27,
                93, 0, 4.65, 5296.11,
                86, 0, 4.35, -3980.70,
                78, -33, 2.75, 52237.69,
                72, -32, 4.50, 55076.47,
                68, 0, 3.23, 261.08,
                64, -10, 1.22, 15773.85,
                46, -16, 0.14, 188491.03,
                38, 0, 3.44, -7756.55,
                37, 0, 4.37, 264.89,
                32, -24, 1.14, 117906.27,
                29, -13, 2.84, 55075.75,
                28, 0, 5.96, -7961.39,
                27, -9, 5.09, 188489.81,
                27, 0, 1.72, 2132.19,
                25, -17, 2.56, 109771.03,
                24, -11, 1.92, 54868.56,
                21, 0, 0.09, 25443.93,
                21, 31, 5.98, -55731.43,
                20, -10, 4.03, 60697.74,
                18, 0, 4.27, 2132.79,
                17, -12, 0.79, 109771.63,
                14, 0, 4.24, -7752.82,
                13, -5, 2.01, 188491.91,
                13, 0, 2.65, 207.81,
                13, 0, 4.98, 29424.63,
                12, 0, 0.93, -7.99,
                10, 0, 2.21, 46941.14,
                10, 0, 3.59, -68.29,
                10, 0, 1.50, 21463.25,
                10, -9, 2.55, 157208.40
                };

                #endregion

                //suncoef = 0;
                // extract data and load arrays

                List<double> sl_ = sdata.Where((x, i) => (i + 4) % 4 == 0).ToList();
                List<double> sr_ = sdata.Where((x, i) => (i + 3) % 4 == 0).ToList();
                List<double> sa_ = sdata.Where((x, i) => (i + 2) % 4 == 0).ToList();
                List<double> sb_ = sdata.Where((x, i) => (i + 1) % 4 == 0).ToList();

                for (int i = 0; i < 50; i++)
                {
                    double w = sa_[i] + sb_[i] * u;
                    dl += sl_[i] * Math.Sin(w);
                    if (sr_[i] != 0)
                    {
                        dr += sr_[i] * Math.Cos(w);
                    }
                }
            }



            dl = Math.IEEERemainder(dl * 0.0000001 + 4.9353929 + 62833.196168 * u, 2.0 * Math.PI);

            dr = 149597870.691 * (dr * 0.0000001 + 1.0001026);

            // geocentric ecliptic position vector of the Sun

            double rlsun = Math.IEEERemainder(dl + 0.0000001 * (-993 + 17 * Math.Cos(3.1 + 62830.14 * u)) + psi, 2.0 * Math.PI);

            double rb = 0;

            // compute declination and right ascension

            double cl = Math.Cos(rlsun);
            double sl = Math.Sin(rlsun);
            double cb = Math.Cos(rb);
            double sb = Math.Sin(rb);

            double decl = Math.Asin(ceps * sb + seps * cb * sl);

            double sra = -seps * sb + ceps * cb * sl;
            double cra = cb * cl;

            double rasc = Math.Atan2(sra, cra);

            // geocentric equatorial position vector of the Sun

            return Tuple.Create(
                dr * Math.Cos(rasc) * Math.Cos(decl),  // rasc            
                dr * Math.Sin(rasc) * Math.Cos(decl),  // decl            
                dr * Math.Sin(decl));                  // rsun
        }

        public static double mltan2raan(double jdate, double mltan, int suncoef, int inutate)
        {
            //  function raan = mltan2raan(jdate, mltan)

            // convert mean local time of the ascending node(MLTAN)
            // to right ascension of the ascending node (RAAN)

            // input

            // jdate = UTC Julian date of ascending node crossing
            // mltan = local time of ascending node(hours)

            // output

            // raan = right ascension of the ascending node(radians)
            // (0 <= raan <= 2 pi)

            // conversion factors

            double dtr = Math.PI / 180.0;
            double atr = dtr / 3600.0;

            // compute apparent right ascension of the sun (radians)

            var res = sun2(jdate, suncoef);
            double rasc_ts = res.Item1;
            double decl = res.Item2;
            double rsun = res.Item3;

            /////////////////////////////
            // equation of time (radians)
            /////////////////////////////

            // mean longitude of the sun(radians)

            double t = (jdate - 2451545) / 365250;

            double t2 = t * t;

            double t3 = t * t * t;

            double t4 = t * t * t * t;

            double t5 = t * t * t * t * t;

            double l0 = dtr * Math.IEEERemainder(280.4664567 + 360007.6982779 * t + 0.03032028 * t2
            + t3 / 49931.0 - t4 / 15299.0 - t5 / 1988000.0, 360.0);

            // nutations

            var rrr = nod(jdate, inutate);
            double psi = rrr.Item1;
            double eps = rrr.Item2;

            // compute mean obliquity of the ecliptic in radians

            t = (jdate - 2451545.0) / 36525.0;

            t2 = t * t;

            t3 = t2 * t;

            double obm = atr * (84381.4480 - 46.8150 * t - 0.00059 * t2 + 0.001813 * t3);

            // compute true obliquity of the ecliptic in radians

            double obt = obm + atr * eps;

            double eot = l0 - dtr * 0.0057183 - rasc_ts + atr * psi * Math.Cos(obt);

            // right ascension of the mean sun(radians)

            double rasc_ms = (rasc_ts + eot) % (2.0 * Math.PI);

            // right ascension of the ascending node
            // based on local mean solar time(radians)

            return (rasc_ms + dtr * 15.0 * (mltan - 12.0)) % (2.0 * Math.PI);  // raan
        }

        public static double RRR(DateTime dt, DateTime dtloc, int suncoef, int inutate)
        {
            // conversion to / from mean local time of the ascending node (MLTAN)
            // and right ascension of the ascending node(RAAN)

            // precision solar ephemeris
            // Meeus equation - of - time algorithm

            // initialize solar ephemeris
            // suncoef = 1;

            // initialize nutation algorithm
            // inutate = 1;

            ///////////////////////////////////
            // request ascending node UTC epoch
            ///////////////////////////////////

            // julian date of ascending node crossing

            double jdate_an = julian(dt.DayOfYear + dt.TimeOfDay.TotalDays, dt.Year);

            // greenwich apparent sidereal time at ascending node

            double gast_an = gast2(jdate_an, 0.0, inutate);


            /////////////////
            // mltan to raan
            ////////////////

            double mltan_an = dtloc.TimeOfDay.TotalHours;// dtloc.Hour + dtloc.Minute / 60.0 + dtloc.Second / 3600.0;

            return mltan2raan(jdate_an, mltan_an, suncoef, inutate);  // raan
        }

        public static Tuple<double, double, double, double, double> funarg(double t)
        {
            //            function[el, elprim, f, d, omega] = funarg(t)

            // this function computes fundamental arguments (mean elements)
            // of the sun and moon.  see seidelmann (1982) celestial
            // mechanics 27, 79 - 106(1980 iau theory of nutation).

            // t = tdb time in julian centuries since j2000.0(in)
            // el = mean anomaly of the moon in radians
            // at date tjd(out)
            // elprim = mean anomaly of the sun in radians
            // at date tjd(out)
            // f = mean longitude of the moon minus mean longitude
            // of the moon's ascending node in radians
            // at date tjd(out)
            // d = mean elongation of the moon from the sun in
            // radians at date tjd(out)
            // omega = mean longitude of the moon's ascending node
            //               in radians at date tjd(out)

            double seccon = 206264.8062470964;

            double rev = 1296000;

            // compute fundamental arguments in arcseconds

            double[] arg = new double[5];

            arg[0] = ((+0.064 * t + 31.310) * t + 715922.633) * t + 485866.733 + Math.IEEERemainder(1325.0 * t, 1.0) * rev;

            arg[0] = Math.IEEERemainder(arg[0], rev);

            arg[1] = ((-0.012 * t - 0.577) * t + 1292581.224) * t + 1287099.8040 + Math.IEEERemainder(99.0 * t, 1.0) * rev;

            arg[1] = Math.IEEERemainder(arg[1], rev);

            arg[2] = ((+0.011 * t - 13.257) * t + 295263.137) * t + 335778.877 + Math.IEEERemainder(1342.0 * t, 1.0) * rev;

            arg[2] = Math.IEEERemainder(arg[2], rev);

            arg[3] = ((+0.019 * t - 6.891) * t + 1105601.328) * t + 1072261.307 + Math.IEEERemainder(1236.0 * t, 1.0) * rev;

            arg[3] = Math.IEEERemainder(arg[3], rev);

            arg[4] = ((0.008 * t + 7.455) * t - 482890.539) * t + 450160.280 - Math.IEEERemainder(5.0 * t, 1.0) * rev;

            arg[4] = Math.IEEERemainder(arg[4], rev);

            // convert arguments to radians

            for (int i = 0; i < 5; i++)
            {
                arg[i] = Math.IEEERemainder(arg[i], rev);

                if (arg[i] < 0.0)
                    arg[i] = arg[i] + rev;


                arg[i] = arg[i] / seccon;
            }

            return Tuple.Create(arg[0], arg[1], arg[2], arg[3], arg[4]);
        }
    }

}
