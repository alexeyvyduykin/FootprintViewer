using SpaceScience.Model;

namespace SpaceScience;

public class SpaceScienceFactory
{
    public Orbit CreateOrbit(double a, double inclDeg)
    {
        double gm = Constants.GM;

        var incl = inclDeg * SpaceMath.DegreesToRadians;

        var period = 2 * Math.PI * Math.Sqrt(a * a * a / gm);

        return new Orbit(a, 0.0, incl, 0.0, 0.0, 0.0, period, new());
    }

    public Orbit CreateOrbit(double a, double inclDeg, double lonANDeg)
    {
        double gm = Constants.GM;

        var incl = inclDeg * SpaceMath.DegreesToRadians;
        var lonAN = lonANDeg * SpaceMath.DegreesToRadians;

        var period = 2 * Math.PI * Math.Sqrt(a * a * a / gm);

        return new Orbit(a, 0.0, incl, 0.0, lonAN, 0.0, period, new());
    }

    public Orbit CreateOrbit(double a, double ecc, double inclDeg, double argOfPerDeg, double lonANDeg, double raanDeg, double period, DateTime epoch)
    {
        var incl = inclDeg * SpaceMath.DegreesToRadians;
        var argOfPer = argOfPerDeg * SpaceMath.DegreesToRadians;
        var lonAN = lonANDeg * SpaceMath.DegreesToRadians;
        var raan = raanDeg * SpaceMath.DegreesToRadians;

        return new Orbit(a, ecc, incl, argOfPer, lonAN, raan, period, epoch);
    }
}
