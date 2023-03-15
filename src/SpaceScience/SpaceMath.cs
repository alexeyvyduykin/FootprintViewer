namespace SpaceScience;

public static class SpaceMath
{
    public static double PI => Math.PI;

    public static double TWOPI => 2.0 * Math.PI;

    public static double HALFPI => Math.PI / 2.0;

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

    public static double FromRadToDeg(double radians) => radians * RadiansToDegrees;

    public static double FromDegToRad(double degrees) => degrees * DegreesToRadians;

    public static (double lonDeg, double latDeg) FromRadToDeg((double lonRad, double latRad) value)
        => (value.lonRad * RadiansToDegrees, value.latRad * RadiansToDegrees);

    public static (double lonRad, double latRad) FromDegToRad((double lonDeg, double latDeg) value)
        => (value.lonDeg * DegreesToRadians, value.latDeg * DegreesToRadians);

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

    public static double LongitudeNormalization(double lon, double left = 0.0, double right = 360.0)
    {
        var res = lon;

        while (res < left)
        {
            res += 360;
        }

        while (res > right)
        {
            res -= 360;
        }

        return res;
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

    public static double Modf(double x, out double y)
    {
        return x - (y = Math.Floor(x));
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