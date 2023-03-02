using FootprintViewer.Data.Models;
using SpaceScience;

namespace FootprintViewer.Data.Builders;

public static class SatelliteBuilder
{
    public static IEnumerable<Satellite> Create(int num)
    {
        var list = new List<Satellite>();

        for (int i = 0; i < num; i++)
        {
            list.Add(CreateSatellite(i));
        }

        return list;
    }

    private static Satellite CreateSatellite(int index)
    {
        var dt = Convert.ToDateTime("1 Jul 2007 12:00:00.000");

        var raan = GetRAAN(dt, 0.0, index * 72.0);

        while (raan > 360.0)
        {
            raan -= 360.0;
        }

        // double a, double ecc, double incl, double argOfPer, double lonAN, double om, double period, DateTime epoch
        return new Satellite()
        {
            Name = $"Satellite{index + 1}",
            Semiaxis = 6945.03,
            Eccentricity = 0.0,
            InclinationDeg = 97.65,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = 0.0,
            RightAscensionAscendingNodeDeg = raan,
            Period = 5760.0,
            Epoch = dt,
            InnerHalfAngleDeg = 32,
            OuterHalfAngleDeg = 48
        };
    }

    private static double GetRAAN(DateTime epoch, double tAN, double lonAN)
    {
        var jd = new Julian(epoch);
        double S = jd.ToGmst();
        //double S = orbitState.SiderealTime();       
        return (tAN * Constants.Omega + S) * ScienceMath.RadiansToDegrees + lonAN;
    }
}
