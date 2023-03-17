using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Model;

namespace FootprintViewer.Data.Builders;

internal static class DefaultSatelliteBuilder
{
    public static IList<Satellite> Create(int num)
    {
        return Enumerable
            .Range(0, num)
            .Select(index => CreateSatellite(index))
            .ToList();
    }

    private static Satellite CreateSatellite(int index)
    {
        var dt = Convert.ToDateTime("1 Jul 2007 12:00:00.000");

        var raan = GetRAAN(dt, 0.0, index * 72.0);

        // double a, double ecc, double incl, double argOfPer, double lonAN, double om, double period, DateTime epoch
        return new Satellite()
        {
            Name = $"Satellite{index + 1}",
            Semiaxis = 6945.03,
            Eccentricity = 0.0,
            InclinationDeg = 97.65,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = 0.0,
            RightAscensionAscendingNodeDeg = SpaceMath.LongitudeNormalization(raan),
            Period = 5760.0,
            Epoch = dt,
            LookAngleDeg = 40, // gam1 = 32, gam2 = 48
            RadarAngleDeg = 16
        };
    }

    private static double GetRAAN(DateTime epoch, double tAN, double lonAN)
    {
        var jd = new Julian(epoch);
        double S = jd.ToGmst();
        //double S = orbitState.SiderealTime();       
        return (tAN * Constants.Omega + S) * SpaceMath.RadiansToDegrees + lonAN;
    }
}
