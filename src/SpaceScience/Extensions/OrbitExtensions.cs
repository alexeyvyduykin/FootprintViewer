using SpaceScience.Model;

namespace SpaceScience.Extensions;

public static class OrbitExtensions
{
    public static (double centralAngleMinDeg, double centralAngleMaxDeg) GetValidRange(this Orbit orbit, double angle1Deg, double angle2Deg)
    {
        double u0 = 0;
        double a = orbit.SemimajorAxis;
        double ecc = orbit.Eccentricity;
        double w = orbit.ArgumentOfPerigee;
        double z1 = (90.0 - angle1Deg) * SpaceMath.DegreesToRadians; //  gam1=20
        double z2 = (90.0 - angle2Deg) * SpaceMath.DegreesToRadians; //  gam2=55

        var gam1 = Math.PI / 2 - z1;
        var gam2 = Math.PI / 2 - z2;
        var ph = a * (1 - ecc * ecc);
        var H = ph / (1 + ecc * Math.Cos(u0 - w)) - Constants.Re;

        double centralAngleMinDeg = (Math.PI / 2 - gam1 - Math.Acos((Constants.Re + H) * Math.Sin(gam1) / Constants.Re)); //2 центр.угол мин.огр полосы обзора
        double centralAngleMaxDeg = (Math.PI / 2 - gam2 - Math.Acos((Constants.Re + H) * Math.Sin(gam2) / Constants.Re)); //2 центр.угол макс.огр.полосы обзора

        centralAngleMinDeg *= SpaceMath.RadiansToDegrees;
        centralAngleMaxDeg *= SpaceMath.RadiansToDegrees;

        return (centralAngleMinDeg, centralAngleMaxDeg);
    }
}
