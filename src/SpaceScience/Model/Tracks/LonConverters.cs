namespace SpaceScience.Model;

public static class LonConverters
{
    public static Func<double, double> Default => s => LonConverter(s);

    private static double LonConverter(double lonDeg)
    {
        while (lonDeg > 180) lonDeg -= 360.0;
        while (lonDeg < -180) lonDeg += 360.0;
        return lonDeg;
    }
}
