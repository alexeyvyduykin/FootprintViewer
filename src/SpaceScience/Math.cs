namespace SpaceScience;

public static class ScienceMath
{
    public static double PI => Math.PI;
    public static double TWOPI => 2.0 * Math.PI;

    public static int Sign(double val)
    {
        if (val > 0.0)
            return 1;
        else if (val < 0.0)
            return -1;
        return 0;
    }

    public static double DegreesToRadians => 0.01745329251994329576;

    public static double RadiansToDegrees => 57.2957795130823208767;

    public static double SecondsToRadians => (PI / 180.0) / 3600.0;

    public static double RadiansToSeconds => 3600.0 / (PI / 180.0);

    public static double DMSToDegrees(double Degrees, double Minutes, double Seconds, bool bPositive = true)
    {
        //validate our parameters
        if (!bPositive)
        {
            //    assert(Degrees >= 0);  //All parameters should be non negative if the "bPositive" parameter is false
            //    assert(Minutes >= 0);
            //    assert(Seconds >= 0);
        }

        if (bPositive)
            return Degrees + Minutes / 60 + Seconds / 3600;
        else
            return -Degrees - Minutes / 60 - Seconds / 3600;
    }

    public static double MapTo0To360Range(double Degrees)
    {
        double fResult = Math.IEEERemainder(Degrees, 360);
        if (fResult < 0)
            fResult += 360;
        return fResult;
    }

    public static bool DoubleEquals(double left, double right, double epsilon)
    {
        return (Math.Abs(left - right) < epsilon);
    }

    public static bool AboutEqual(double x, double y)
    {
        double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
        return Math.Abs(x - y) <= epsilon;
    }

    public static bool AboutLess(this double value, double compare)
    {
        if (AboutEqual(value, compare))
        {
            return false;
        }
        return value < compare;
    }

    public static bool AboutLessOrEqual(this double value, double compare)
    {
        if (AboutEqual(value, compare))
        {
            return true;
        }
        return value < compare;
    }

    public static bool AboutGreater(this double value, double compare)
    {
        if (AboutEqual(value, compare))
        {
            return false;
        }
        return value > compare;
    }

    public static bool AboutGreaterOrEqual(this double value, double compare)
    {
        if (AboutEqual(value, compare))
        {
            return true;
        }
        return value > compare;
    }

    public static bool InRange(double value, double left, double right)
    {
        if ((value.AboutGreaterOrEqual(left) && value.AboutLessOrEqual(right)) ||
            (value.AboutLessOrEqual(left) && value.AboutGreaterOrEqual(right)))
            return true;
        return false;
    }

    //public static bool DoubleLess(double left, double right, double epsilon, bool orequal)
    //{
    //    //if (Math.Abs(left - right) < epsilon)
    //    //{
    //    //    // В рамках epsilon, так что считаются равными
    //    //    return (orequal);
    //    //}

    //    if (AboutEqual(left, right))
    //    {
    //        return (orequal);
    //    }

    //    return (left < right);
    //}

    //public static bool DoubleGreater(double left, double right, double epsilon, bool orequal)
    //{
    //    //if (Math.Abs(left - right) < epsilon)
    //    //{
    //    //    // В рамках epsilon, так что считаются равными
    //    //    return (orequal);
    //    //}

    //    if (AboutEqual(left, right))
    //    {
    //        return (orequal);
    //    }

    //    return (left > right);
    //}

    //public static bool InRange(double value, double left, double right, double eps = 0.000001)
    //{
    //    if ((DoubleGreater(value, left, eps, true) && DoubleLess(value, right, eps, true)) ||
    //        (DoubleLess(value, left, eps, true) && DoubleGreater(value, right, eps, true))) return true;
    //    return false;
    //}



    //public static bool InRange(double value, double minimum, double maximum)
    //{
    //    if (value.CompareTo(minimum) < 0)
    //        return false;
    //    if (value.CompareTo(maximum) > 0)
    //        return false;
    //    return true;
    //}

    private static int Sgn(double val)
    {
        if (Math.Abs(val) <= 1e-9)
            return 1;
        if (val < 0)
            return -1;
        return 1;
    }

    private static double Sqr(double val)
    {
        return val * val;
    }

    private static double ArcCos_(double val)
    {
        if (val == 0.0)
            return Math.PI / 2.0;
        if (Math.Abs(val) > 1.0)
            return (Math.PI / 2.0) * (1 - Sgn(val));
        double z = Math.Atan(Math.Sqrt(1 - Sqr(val)) / Math.Abs(val));
        if (val < 0)
            return Math.PI - z;
        else
            return z;
    }

    public static double ArcCos2(double Sinx, double Cosx)
    {
        double z = ArcCos_(Cosx);
        if (Sinx < 0)
            z = 2.0 * Math.PI - z;
        return z;
    }

    public static double Modf(double __x, out double __y)
    {
        return __x - (__y = Math.Floor(__x));
    }

    public static double Round(double x)
    {
        double d = x - Math.Floor(x);
        if (d <= 0.5)
            return Math.Floor(x);
        else
            return Math.Ceiling(x);
    }

    public static void Swap(ref double x, ref double y)
    {
        (y, x) = (x, y);
    }


    public static double WrapAngle(double angle)
    {
        while (angle > 2.0 * Math.PI)
            angle -= 2.0 * Math.PI;
        while (angle < 0)
            angle += 2.0 * Math.PI;
        return angle;
    }

    public static double WrapAngle360(double angle)
    {
        while (angle > 360.0)
            angle -= 360.0;
        while (angle < 0.0)
            angle += 360.0;
        return angle;
    }

    //public static double WrapAngle(double angle)
    //{
    //    angle = Math.IEEERemainder(angle, 2.0 * Math.PI);
    //    if (angle < 0.0)
    //        angle += 2.0 * Math.PI;
    //    return angle;
    //}
}

public static class ScienceConverters
{
    public static Geo2D From0To360(Geo2D point)
    {
        if (point.Type == GeoCoordTypes.Radians)
        {
            var lon = point.Lon;
            while (lon > 2.0 * Math.PI)
            {
                lon -= 2.0 * Math.PI;
            }
            while (lon < 0.0)
            {
                lon += 2.0 * Math.PI;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Radians);
        }
        else
        {
            var lon = point.Lon;
            while (lon > 360.0)
            {
                lon -= 360.0;
            }
            while (lon < 0.0)
            {
                lon += 360.0;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Degrees);
        }

    }

    public static Geo2D From180To180(Geo2D point)
    {
        if (point.Type == GeoCoordTypes.Radians)
        {
            var lon = point.Lon;
            while (lon > Math.PI)
            {
                lon -= 2.0 * Math.PI;
            }
            while (lon < -Math.PI)
            {
                lon += 2.0 * Math.PI;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Radians);
        }
        else
        {
            var lon = point.Lon;
            while (lon > 180.0)
            {
                lon -= 360.0;
            }
            while (lon < -180.0)
            {
                lon += 360.0;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Degrees);
        }
    }
}

static class MyFunction
{

    public static double DateToMJD(int Year, int Month, int Day)
    {
        double Var1, Var2, Var3;
        Var1 = 10000 * Year + 100 * Month + Day;
        if (Month <= 2)
        {
            Month += 12;
            Year -= 1;
        }
        if (Var1 <= 15821004)
            Var2 = -2.0 + Math.Floor((double)(Year + 4716.0) / 4.0) - 1179.0;
        else
            Var2 = Math.Floor((double)Year / 400.0) - Math.Floor((double)Year / 100.0) + Math.Floor((double)Year / 4.0);
        Var3 = 365.0 * Year - 679004.0;
        // MJD - Модифицированная Юлианская дата
        return Var3 + Var2 + Math.Floor(306001.0 * (double)(Month + 1.0) / 10000.0) + Day;
    }

    // Calculate the Julian Day (JD),
    public static double DateToJD(int Year, int Month, int Day)
    {
        int Y = Year;
        int M = Month;
        if (M < 3)
        {
            Y -= 1;
            M += 12;
        }
        int A = (int)(Y / 100.0);
        int B = 2 - A + (int)(A / 4.0);
        return (int)(365.25 * (Y + 4716)) + (int)(30.6001 * (M + 1)) + Day + B - 1524.5;
    }

    // Calculate the Julian Ephemeris Day(JDE)    
    public static double ToJDE(double JD, double tSec)
    {
        return JD + tSec / 86400.0;
    }

    // Calculate the Julian century(JC) for the 2000 standard epoch 
    public static double ToJC(double JD)
    {
        return (JD - 2451545.0) / 36525.0;
    }

    // Calculate the Julian Ephemeris Century(JCE) for the 2000 standard epoch
    public static double ToJCE(double JDE)
    {
        return (JDE - 2451545.0) / 36525.0;
    }

    // Calculate the Julian Ephemeris Millennium (JME) for the 2000 standard epoch
    public static double ToJME(double JCE)
    {
        return JCE / 10.0;
    }

    public static void Sun(double ud, double[] rs)
    {
        double d, t, e, r, v, h;
        d = ud - 2415020.0;
        t = d / 36525.0;
        e = 0.1675104e-1 - (0.418e-4 + 0.126e-6 * t) * t;
        r = 6.25658378411 + 1.72019697677e-2 * d - 2.61799387799e-6 * t * t;
        v = 4.90822940869 + 8.21498553644e-7 * d + 7.90634151156e-6 * t * t;
        r = r + 2.0 * e * Math.Sin(r) + 1.25 * e * e * Math.Sin(2.0 * r);
        h = 4.09319747446e-1 - (2.27110968916e-4 + (2.86233997327e-8 -
          8.77900613756e-9 * t) * t) * t + 4.46513400244e-5 * Math.Cos(4.52360151485 -
          (3.37571462465e+1 - (3.62640633471e-5 + 3.87850944887e-8 * t) * t) * t);
        rs[3] = 149600034.408 * (1.0 - e * e) / (1.0 + e * Math.Cos(r));
        v += r;
        r = Math.Sin(v);
        rs[0] = rs[3] * Math.Cos(v);
        rs[1] = rs[3] * r * Math.Cos(h);
        rs[2] = rs[3] * r * Math.Sin(h);
    }

    public static bool MY_SUN(double ud0h, double s0, double t, double a, double lonSatRAD, double latSatRAD, double hSunMin)
    {
        double S = s0 + Constants.Omega * t;
        double LA = S + lonSatRAD;
        while (LA > Math.PI / 2.0)
            LA -= Math.PI / 2.0;

        double xSat = a * Math.Cos(latSatRAD) * Math.Cos(LA);
        double ySat = a * Math.Cos(latSatRAD) * Math.Sin(LA);
        double zSat = a * Math.Sin(latSatRAD);

        double ud = ud0h + t / 86400.0;

        //sun[x_sun,y_sun,z_sun,r_sun,S0,S]
        double[] rs = new double[4];
        Sun(ud, rs);
        double xSun = rs[0];
        double ySun = rs[1];
        double zSun = rs[2];

        double dltob = Math.Acos((xSat * xSun + ySat * ySun + zSat * zSun) / (Math.Sqrt(xSat * xSat + ySat * ySat + zSat * zSat) * Math.Sqrt(xSun * xSun + ySun * ySun + zSun * zSun)));

        if (dltob >= 0.0 && dltob <= (Math.PI / 2.0 - hSunMin))
            return true;

        return false;
    }

    public static bool MY_SUN_TEST_MODULE(double jd0h, double s0, double tcur, double r, double lon, double lat)
    {
        double[] rs = new double[4];
        double jd = jd0h + tcur / 86400.0;
        Sun(jd, rs);

        double xSunNorm = rs[0] / rs[3];
        double ySunNorm = rs[1] / rs[3];
        double zSunNorm = rs[2] / rs[3];

        double S = s0 + Constants.Omega * tcur;
        double LA = S + lon;
        while (LA > 2.0 * Math.PI)
            LA -= 2.0 * Math.PI;

        double xSat = r * Math.Cos(lat) * Math.Cos(LA);
        double ySat = r * Math.Cos(lat) * Math.Sin(LA);
        double zSat = r * Math.Sin(lat);

        double rSat = Math.Sqrt(xSat * xSat + ySat * ySat + zSat * zSat);
        double rSun = Math.Sqrt(xSunNorm * xSunNorm + ySunNorm * ySunNorm + zSunNorm * zSunNorm);
        double r_S = xSat * xSunNorm + ySat * ySunNorm + zSat * zSunNorm;


        double angle = Math.Acos(r_S / (rSat * rSun));

        r_S = Math.Abs(r_S);

        if (angle > Math.PI / 2.0)
            r_S = -r_S;

        double G = r_S + Math.Sqrt(rSat * rSat - Constants.Re * Constants.Re);

        if (G < 0.0)
            return false;
        return true;
    }


    //   31 Dec 1899 12:00:00.000 UTCG

    //public static double uds1900(double jd)
    //{
    //    // Дублинский юлианский день(DJD) 12:00 31 декабря, 1899, Вс (DJD = JD − 2415020)                              
    //    double t1 = (jd - 2415020.0);
    //    double s = 1.73993589372 + 0.0172027912658 * t1 + (0.6755878e-5) * (t1 / 36525) * (t1 / 36525);
    //    s = Math.IEEERemainder(s, 2.0 * Math.PI);// fmod(s, 6.283185307179586);
    //    return s;
    //}

    //public static double uds2000(double jd)
    //{
    //    double t1 = (jd - 2451545.0);
    //    double s = 1.73993589372 + 0.0172027912658 * t1 + (0.6755878e-5) * (t1 / 36525) * (t1 / 36525);
    //    s = Math.IEEERemainder(s, 2.0 * Math.PI);
    //    return s;
    //}

    //// MGST - Mean Greenwich Sidereal Time
    //public static double MGST(DateTime date)
    //{
    //    double JDMidnight = DateToJD(date.Year, date.Month, date.Day);

    //    //Calculate the sidereal time at midnight
    //    double T = (JDMidnight - 2451545.0) / 36525;
    //    double TSquared = T * T;
    //    double TCubed = TSquared * T;
    //    double Value = 100.46061837 + (36000.770053608 * T) + (0.000387933 * TSquared) - (TCubed / 38710000);

    //    //Adjust by the time of day
    //    Value += (((date.Hour * 15) + (date.Minute * 0.25) + (date.Second * 0.0041666666666666666666666666666667)) * 1.00273790935);

    //    Value = MyMath.WrapAngle360(Value);
    //    return Value;
    //}

    //private static double[][] _koef_id = new double[][]   /// [30][5]
    //{
    //    new double[]{ 0.0, 0.0, 0.0, 0.0, 1.0 },  //1   
    //    new double[]{ 0.0, 0.0, 0.0, 0.0, 2.0},  //2
    //    new double[] { -2.0, 0.0, 2.0, 0.0, 1.0},  //3
    //    new double[] { 2.0, 0.0,-2.0, 0.0, 0.0},
    //    new double[]{ -2.0, 0.0, 2.0, 0.0, 2.0},
    //    new double[]{ 1.0,-1.0, 0.0,-1.0, 0.0},  //6
    //    new double[]{ 0.0,-2.0, 2.0,-2.0, 1.0},
    //    new double[]{ 2.0, 0.0,-2.0, 0.0, 1.0},
    //    new double[]{ 0.0, 0.0, 2.0,-2.0, 2.0},  //9
    //    new double[]{ 0.0,-1.0, 0.0, 0.0, 0.0},
    //    new double[]{ 0.0, 1.0, 2.0,-2.0, 2.0},  //11
    //    new double[]{ 0.0,-1.0, 2.0,-2.0, 2.0},
    //    new double[]{ 0.0, 0.0, 2.0,-2.0, 1.0},  //13
    //    new double[]{ -2.0, 0.0, 0.0, 2.0, 0.0},  //14
    //    new double[]{ 0.0, 0.0, 2.0,-2.0, 0.0},
    //    new double[]{ 0.0, 2.0, 0.0, 0.0, 0.0},
    //    new double[]{ 0.0, 1.0, 0.0, 0.0, 1.0},  //17
    //    new double[]{ 0.0, 2.0, 2.0,-2.0, 2.0},
    //    new double[]{ 0.0,-1.0, 0.0, 0.0, 1.0},  //19
    //    new double[]{ -2.0, 0.0, 0.0, 2.0, 1.0},
    //    new double[]{ 0.0,-1.0, 2.0,-2.0, 1.0},  //21
    //    new double[]{ 2.0, 0.0, 0.0,-2.0, 1.0},
    //    new double[]{ 0.0, 1.0, 2.0,-2.0, 1.0},  //23
    //    new double[]{ 1.0, 0.0, 0.0,-1.0, 0.0},
    //    new double[]{ 2.0, 1.0, 0.0,-2.0, 0.0},  //25
    //    new double[]{ 0.0, 0.0,-2.0, 2.0, 1.0},
    //    new double[]{ 0.0, 1.0,-2.0, 2.0, 0.0},
    //    new double[]{  0.0, 1.0, 0.0, 0.0, 2.0},  //28
    //    new double[]{ -1.0, 0.0, 0.0, 1.0, 1.0},
    //    new double[]{ 0.0, 1.0, 2.0,-2.0, 0.0}
    //}; //30

    //private static double[][] _koef_abd = new double[][]         // [30][4]
    //{
    //    new double[]{-171996.0,-174.2, 92025.0, 8.9 }, //1
    //    new double[]{2062.0,   0.2,  -895.0, 0.5 },
    //    new double[]{46.0,   0.0,   -24.0, 0.0 }, //3
    //    new double[]{11.0,   0.0,     0.0, 0.0 },
    //    new double[]{-3.0,   0.0,     1.0, 0.0 },
    //    new double[]{-3.0,   0.0,     0.0, 0.0 }, //6
    //    new double[]{-2.0,   0.0,     1.0, 0.0 },
    //    new double[]{1.0,   0.0,     1.0, 0.0 }, //8
    //    new double[]{-13187.0,  -1.6,  5736.0,-3.1 },
    //    new double[]{-1426.0,   3.4,    54.0,-0.1 }, //10
    //    new double[]{-517.0,   1.2,   224.0,-0.6 },
    //    new double[]{217.0,  -0.5,   -95.0, 0.3 }, //12
    //    new double[]{ 129.0,   0.1,   -70.0, 0.0 },
    //    new double[]{-48.0,   0.0,     1.0, 0.0 }, //14
    //    new double[]{-22.0,   0.0,     0.0, 0.0 },
    //    new double[]{ 17.0,  -0.1,     0.0, 0.0 }, //16
    //    new double[]{-15.0,   0.0,     9.0, 0.0 },
    //    new double[]{-16.0,   0.1,     7.0, 0.0 },
    //    new double[]{-12.0,   0.0,     6.0, 0.0 }, //19
    //    new double[]{-6.0,   0.0,     3.0, 0.0 },
    //    new double[]{-5.0,   0.0,     3.0, 0.0 }, //21
    //    new double[]{4.0,   0.0,    -2.0, 0.0 },
    //    new double[]{4.0,   0.0,    -2.0, 0.0 },
    //    new double[]{-4.0,   0.0,     0.0, 0.0 }, //24
    //    new double[]{1.0,   0.0,     0.0, 0.0 },
    //    new double[]{1.0,   0.0,     0.0, 0.0 },
    //    new double[]{-1.0,   0.0,     0.0, 0.0 }, //27                            
    //    new double[]{1.0,   0.0,     0.0, 0.0 },
    //    new double[]{1.0,   0.0,     0.0, 0.0 }, //29        
    //    new double[]{-1.0,   0.0,     0.0, 0.0 }
    //};

    //private static double[][] _koef_ik = new double[][]      // [76][5]            
    //{
    //    new double[]{0.0, 0.0, 2.0, 0.0, 2.0}, //31
    //    new double[]{1.0, 0.0, 0.0, 0.0, 0.0},
    //    new double[]{0.0, 0.0, 2.0, 0.0, 1.0}, //33
    //    new double[]{1.0, 0.0, 2.0, 0.0, 2.0},
    //    new double[]{1.0, 0.0, 0.0,-2.0, 0.0}, //35
    //    new double[]{-1.0, 0.0, 2.0, 0.0, 2.0},
    //    new double[]{0.0, 0.0, 0.0, 2.0, 0.0},
    //    new double[]{1.0, 0.0, 0.0, 0.0, 1.0}, //38
    //    new double[]{-1.0, 0.0, 0.0, 0.0, 1.0},
    //    new double[]{-1.0, 0.0, 2.0, 2.0, 2.0}, //40
    //    new double[]{1.0, 0.0, 2.0, 0.0, 1.0},
    //    new double[]{0.0, 0.0, 2.0, 2.0, 2.0},
    //    new double[]{2.0, 0.0, 0.0, 0.0, 0.0}, //43
    //    new double[]{1.0, 0.0, 2.0,-2.0, 2.0},
    //    new double[]{2.0, 0.0, 2.0, 0.0, 2.0},
    //    new double[]{0.0, 0.0, 2.0, 0.0, 0.0}, //46
    //    new double[]{-1.0, 0.0, 2.0, 0.0, 1.0},
    //    new double[]{-1.0, 0.0, 0.0, 2.0, 1.0},
    //    new double[]{1.0, 0.0, 0.0,-2.0, 1.0}, //49
    //    new double[]{-1.0, 0.0, 2.0, 2.0, 1.0},
    //    new double[]{ 1.0, 1.0, 0.0,-2.0, 0.0},
    //    new double[]{ 0.0, 1.0, 2.0, 0.0, 2.0}, //52
    //    new double[]{ 0.0,-1.0, 2.0, 0.0, 2.0},
    //    new double[]{ 1.0, 0.0, 2.0, 2.0, 2.0}, //54
    //    new double[]{ 1.0, 0.0, 0.0, 2.0, 0.0},
    //    new double[]{ 2.0, 0.0, 2.0,-2.0, 2.0}, //56
    //    new double[]{0.0, 0.0, 0.0, 2.0, 1.0},
    //    new double[]{0.0, 0.0, 2.0, 2.0, 1.0},
    //    new double[]{ 1.0, 0.0, 2.0,-2.0, 1.0}, //59
    //    new double[]{0.0, 0.0, 0.0,-2.0, 1.0},
    //    new double[]{1.0,-1.0, 0.0, 0.0, 0.0},
    //    new double[]{2.0, 0.0, 2.0, 0.0, 1.0}, //62
    //    new double[]{0.0, 1.0, 0.0,-2.0, 0.0},
    //    new double[]{1.0, 0.0,-2.0, 0.0, 0.0},
    //    new double[]{0.0, 0.0, 0.0, 1.0, 0.0}, //65
    //    new double[]{1.0, 1.0, 0.0, 0.0, 0.0},
    //    new double[]{1.0, 0.0, 2.0, 0.0, 0.0}, //67
    //    new double[]{ 1.0,-1.0, 2.0, 0.0, 2.0},
    //    new double[]{-1.0,-1.0, 2.0, 2.0, 2.0},
    //    new double[]{-2.0, 0.0, 0.0, 0.0, 1.0}, //70
    //    new double[]{ 3.0, 0.0, 2.0, 0.0, 2.0},
    //    new double[]{0.0,-1.0, 2.0, 2.0, 2.0}, //72
    //    new double[]{ 1.0, 1.0, 2.0, 0.0, 2.0},
    //    new double[]{-1.0, 0.0, 2.0,-2.0, 1.0},
    //    new double[]{ 2.0, 0.0, 0.0, 0.0, 1.0}, //75
    //    new double[]{1.0, 0.0, 0.0, 0.0, 2.0},
    //    new double[]{ 3.0, 0.0, 0.0, 0.0, 0.0},
    //    new double[]{ 0.0, 0.0, 2.0, 1.0, 2.0}, //78
    //    new double[]{-1.0, 0.0, 0.0, 0.0, 2.0},
    //    new double[]{ 1.0, 0.0, 0.0,-4.0, 0.0}, //80
    //    new double[]{-2.0, 0.0, 2.0, 2.0, 2.0},
    //    new double[]{-1.0, 0.0, 2.0, 4.0, 2.0},
    //    new double[]{ 2.0, 0.0, 0.0,-4.0, 0.0}, //83
    //    new double[]{1.0, 1.0, 2.0,-2.0, 2.0},
    //    new double[]{ 1.0, 0.0, 2.0, 2.0, 1.0},
    //    new double[]{-2.0, 0.0, 2.0, 4.0, 2.0}, //86
    //    new double[]{-1.0, 0.0, 4.0, 0.0, 2.0},
    //    new double[]{1.0,-1.0, 0.0,-2.0, 0.0},
    //    new double[]{ 2.0, 0.0, 2.0,-2.0, 1.0},
    //    new double[]{ 2.0, 0.0, 2.0, 2.0, 2.0}, //90
    //    new double[]{ 1.0, 0.0, 0.0, 2.0, 1.0},
    //    new double[]{0.0, 0.0, 4.0,-2.0, 2.0},
    //    new double[]{ 3.0, 0.0, 2.0,-2.0, 2.0}, //93
    //    new double[]{ 1.0, 0.0, 2.0,-2.0, 0.0},
    //    new double[]{ 0.0, 1.0, 2.0, 0.0, 1.0},
    //    new double[]{-1.0,-1.0, 0.0, 2.0, 1.0},
    //    new double[]{ 0.0, 0.0,-2.0, 0.0, 1.0}, //97
    //    new double[]{ 0.0, 0.0, 2.0,-1.0, 2.0},
    //    new double[]{0.0, 1.0, 0.0, 2.0, 0.0},
    //    new double[]{1.0, 0.0,-2.0,-2.0, 0.0}, //100
    //    new double[]{ 0.0,-1.0, 2.0, 0.0, 1.0},
    //    new double[]{ 1.0, 1.0, 0.0,-2.0, 1.0},
    //    new double[]{ 1.0, 0.0,-2.0, 2.0, 0.0}, //103
    //    new double[]{ 2.0, 0.0, 0.0, 2.0, 0.0},
    //    new double[]{ 0.0, 0.0, 2.0, 4.0, 2.0},
    //    new double[]{ 0.0, 1.0, 0.0, 1.0, 0.0}
    //}; //106

    //private static double[][] _koef_abk = new double[][]            // [76][4]        
    //{
    //    new double[]{-2274.0,-0.2, 977.0,-0.5 }, // 31
    //    new double[]{712.0, 0.1,  -7.0, 0.0 },
    //    new double[]{-386.0,-0.4, 200.0, 0.0 },
    //    new double[]{-301.0, 0.0, 129.0,-0.1 }, // 34
    //    new double[]{-158.0, 0.0,  -1.0, 0.0 },
    //    new double[]{123.0, 0.0, -53.0, 0.0 },
    //    new double[]{63.0, 0.0,  -2.0, 0.0 }, // 37
    //    new double[]{63.0, 0.1, -33.0, 0.0 },
    //    new double[]{-58.0,-0.1,  32.0, 0.0 },
    //    new double[]{-59.0, 0.0,  26.0, 0.0 }, // 40                        
    //    new double[]{-51.0, 0.0,  27.0, 0.0 },
    //    new double[]{-38.0, 0.0,  16.0, 0.0 },
    //    new double[]{ 29.0, 0.0,  -1.0, 0.0 }, // 43
    //    new double[]{29.0, 0.0, -12.0, 0.0 },
    //    new double[]{-31.0, 0.0,  13.0, 0.0 },
    //    new double[]{ 26.0, 0.0,  -1.0, 0.0 }, // 46
    //    new double[]{ 21.0, 0.0, -10.0, 0.0 },
    //    new double[]{ 16.0, 0.0,  -8.0, 0.0 },
    //    new double[]{-13.0, 0.0,   7.0, 0.0 }, // 49
    //    new double[]{-10.0, 0.0,   5.0, 0.0 },
    //    new double[]{-7.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 7.0, 0.0,  -3.0, 0.0 }, // 52
    //    new double[]{-7.0, 0.0,   3.0, 0.0 },
    //    new double[]{-8.0, 0.0,   3.0, 0.0 },
    //    new double[]{ 6.0, 0.0,   0.0, 0.0 }, // 55
    //    new double[]{ 6.0, 0.0,  -3.0, 0.0 },
    //    new double[]{-6.0, 0.0,   3.0, 0.0 },
    //    new double[]{-7.0, 0.0,   3.0, 0.0 }, // 58
    //    new double[]{ 6.0, 0.0,  -3.0, 0.0 },
    //    new double[]{-5.0, 0.0,   3.0, 0.0 },
    //    new double[]{ 5.0, 0.0,   0.0, 0.0 }, // 61
    //    new double[]{-5.0, 0.0,   3.0, 0.0 },
    //    new double[]{-4.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 4.0, 0.0,   0.0, 0.0 }, // 64
    //    new double[]{-4.0, 0.0,   0.0, 0.0 },
    //    new double[]{-3.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 3.0, 0.0,   0.0, 0.0 }, // 67
    //    new double[]{-3.0, 0.0,   1.0, 0.0 },
    //    new double[]{-3.0, 0.0,   1.0, 0.0 },
    //    new double[]{-2.0, 0.0,   1.0, 0.0 }, // 70
    //    new double[]{-3.0, 0.0,   1.0, 0.0 },
    //    new double[]{-3.0, 0.0,   1.0, 0.0 },
    //    new double[]{ 2.0, 0.0,  -1.0, 0.0 }, // 73
    //    new double[]{-2.0, 0.0,   1.0, 0.0 },
    //    new double[]{ 2.0, 0.0,  -1.0, 0.0 },
    //    new double[]{-2.0, 0.0,   1.0, 0.0 }, // 76
    //    new double[]{ 2.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 2.0, 0.0,  -1.0, 0.0 },
    //    new double[]{ 1.0, 0.0,  -1.0, 0.0 }, // 79
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,  -1.0, 0.0 },
    //    new double[]{-2.0, 0.0,   1.0, 0.0 }, // 82
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,  -1.0, 0.0 },
    //    new double[]{-1.0, 0.0,   1.0, 0.0 }, // 85
    //    new double[]{-1.0, 0.0,   1.0, 0.0 },
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 }, // 88
    //    new double[]{ 1.0, 0.0,  -1.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 }, // 91
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 }, // 94
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 }, // 97
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 }, // 100
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 }, // 103
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 },
    //    new double[]{-1.0, 0.0,   0.0, 0.0 },
    //    new double[]{ 1.0, 0.0,   0.0, 0.0 }
    //}; // 106

    //public static double utc_nut(double t)
    //{
    //    //double t= 6.023472005475702e+002;
    //    double R = 1296000.0; // li= 360grad= 1,296,000 cek
    //    double t2 = t * t;
    //    double t3 = t2 * t;
    //    double l = 485866.733 + (1325.0 * R + 715922.633) * t + 31.310 * t2 + 0.064 * t3;
    //    double l1 = 1287099.804 + (99.0 * R + 1292581.224) * t - 0.577 * t2 - 0.012 * t3;
    //    double f = 335778.877 + (1342.0 * R + 295263.137) * t - 13.257 * t2 + 0.011 * t3;
    //    double dd = 1072261.307 + (1236.0 * R + 1105601.328) * t - 6.891 * t2 + 0.019 * t3;
    //    double omega = 450160.280 - (5.0 * R + 482890.539) * t + 7.455 * t2 + 0.008 * t3;
    //    double eps0 = 84381.448 - 46.8150 * t - 0.00059 * t2 + 0.001813 * t3;
    //    double eps_d = utc_nut_fi_eps(t, l, l1, f, dd, omega, "d", "e"); // n=30
    //    double eps_k = utc_nut_fi_eps(t, l, l1, f, dd, omega, "k", "e"); // n=76
    //    double eps = eps0 + eps_d + eps_k;
    //    double cos_eps = Math.Cos(MyMath.RAD_SEK_ANGL * eps) / 15.0;
    //    double d_fi = utc_nut_fi_eps(t, l, l1, f, dd, omega, "d", "f"); // n=30
    //    double k_fi = utc_nut_fi_eps(t, l, l1, f, dd, omega, "k", "f"); // n=76
    //    double nut1 = d_fi * cos_eps;
    //    double nut2 = k_fi * cos_eps;
    //    double nut3 = 0.0;// = 0.00264*sin(omega)+0.000063*sin(2.*omega);
    //                      //double nut1_dop= nut1;
    //                      //double nut2_dop= nut2;
    //                      //double nut3_dop= nut3;
    //    double nut = nut1 + nut2 + nut3;
    //    return nut;
    //}

    //public static double utc_nut_fi_eps(double t, double l, double l1, double f, double dd, double omega, string typ_nut, string fi_eps)
    //{
    //    int n;
    //    double s1, a, bt, sa, sb;
    //    if (typ_nut[0] == 'd') n = 30;
    //    else n = 76;
    //    double sum_a = 0.0;
    //    double sum_b = 0.0;
    //    for (int i = 0; i < n; i++)
    //    {
    //        if (typ_nut[0] == 'd')
    //        {
    //            s1 = koef_id[i][0] * l + koef_id[i][1] * l1 + koef_id[i][2] * f +
    //                koef_id[i][3] * dd + koef_id[i][4] * omega;
    //            if (fi_eps[0] == 'f')
    //            {
    //                a = koef_abd[i][0] * 1e-4;
    //                bt = koef_abd[i][1] * 1e-4;
    //            }
    //            else
    //            {
    //                a = koef_abd[i][2] * 1e-4;
    //                bt = koef_abd[i][3] * 1e-4;
    //            }
    //        }
    //        else
    //        {
    //            s1 = koef_ik[i][0] * l + koef_ik[i][1] * l1 + koef_ik[i][2] * f +
    //                koef_ik[i][3] * dd + koef_ik[i][4] * omega;
    //            if (fi_eps[0] == 'f')
    //            {
    //                a = koef_abk[i][0] * 1e-4;
    //                bt = koef_abk[i][1] * 1e-4;
    //            }
    //            else
    //            {
    //                a = koef_abk[i][2] * 1e-4;
    //                bt = koef_abk[i][3] * 1e-4;
    //            }
    //        }

    //        if (fi_eps[0] == 'f')
    //        {
    //            double sin_s1 = Math.Sin(MyMath.RAD_SEK_ANGL * s1);
    //            sa = a * sin_s1;
    //            sb = bt * sin_s1;
    //        }
    //        else
    //        {
    //            double cos_s1 = Math.Cos(MyMath.RAD_SEK_ANGL * s1);
    //            sa = a * cos_s1;
    //            sb = bt * cos_s1;
    //        }
    //        //double arg= RAD_SEK_ANGL*s1;
    //        sum_a = sum_a + sa;
    //        sum_b = sum_b + sb;
    //    }
    //    double nut_fi_eps = sum_a + sum_b * t;
    //    return nut_fi_eps;
    //}

    public static bool ChopCI(ref double tIn, ref double tOut, double numer, double denom)
    {
        double tHit;
        if (denom < 0)                           // луч входит
        {
            tHit = numer / denom;
            if (tHit > tOut)
                return false;    // досрочный выход
            else
            { if (tHit > tIn) { tIn = tHit; } }  // берём больше t
        }
        else if (denom > 0)                   // луч выходит
        {
            tHit = numer / denom;
            if (tHit < tIn)
                return false;        // досрочный выход
            else
            { if (tHit < tOut) { tOut = tHit; } } // берём меньшее t
        }
        else
        {                 // denom(знаменатель) равен нулю: луч параллелен
            if (numer <= 0)
            {
                return false;
            }
        }                     // прошёл мимо прямой
        return true;             // возможный интервал по-прежнему пуст
    }

    public static bool CutSegments(double A, double B, ref double C, ref double D)
    {
        // Функция деления отрезка CD, AB - секущая
        // 0 - отрезок CD не лежит или не касается AB
        // 1 - отрезок CD ли его часть лежит на AB

        if ((B < C) || (D < A))
            return false;
        if (C < A)
            C = A;
        if (B < D)
            D = B;
        return true;
    }

    public static bool PrdctJoinSegments(double A, double B, ref double C, ref double D)
    {
        // Функция объединения двух отрезков
        // 0 - отрезки не пересекаются
        // 1 - отрезки пересекаются, в &C и &D записывается новый отрезок

        // прямые не пересекаются
        if ((B < C) || (D < A))          // случай №1: A-----B  C------D
        {
            return false;
        }
        // прямые касаются вершиной
        if ((B == C) || (D == A))       // случай №2: A------BC------D
        {
            if (B == C)
                C = A;
            if (D == A)
                D = B;
            return true;
        }
        // прямые пересекаются или совпадают
        if ((B > C) || (D > A))         // случай №3: A-----C==B------D,  AC=====BD
        {
            if ((B > C) && (C > A))
                C = A;
            if ((D > A) && (D < B))
                D = B;
            return true;
        }
        return false;
    }
}
