namespace SpaceScience.Model;

public class FactorShiftTrack
{
    public FactorShiftTrack(Orbit orbit, double gam1DEG, double gam2DEG, SwathDirection direction)
    {
        double gam1 = gam1DEG * SpaceMath.DegreesToRadians;
        double gam2 = gam2DEG * SpaceMath.DegreesToRadians;

        var (pls1, pls2) = direction switch
        {
            SwathDirection.Middle => (-1, 1),
            SwathDirection.Left => (-1, -1),
            SwathDirection.Right => (1, 1),
            _ => throw new NotImplementedException()
        };

        double u_90 = SpaceMath.HALFPI;
        double u_270 = 3.0 * SpaceMath.HALFPI;

        double semi_axis = orbit.Semiaxis(SpaceMath.HALFPI);

        double fi1 = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(gam1) / Constants.Re) - gam1;
        double fi2 = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(gam2) / Constants.Re) - gam2;

        double i1_90 = orbit.Inclination - fi1 * pls1;
        double i2_90 = orbit.Inclination - fi2 * pls2;

        double di1_90 = Math.Abs(u_90 - i1_90);
        double di2_90 = Math.Abs(u_90 - i2_90);

        double i1_270 = Math.PI + orbit.Inclination + fi1 * pls1;
        double i2_270 = Math.PI + orbit.Inclination + fi2 * pls2;

        double di1_270 = Math.Abs(u_270 - i1_270);
        double di2_270 = Math.Abs(u_270 - i2_270);

        var quart1 = orbit.TimeHalfPi();
        var quart3 = orbit.Period - orbit.TimeHalfPi();

        double lon1 = GetLongitude(orbit, u_90, fi1, pls1) - Constants.Omega * quart1;// Period / 4.0;
        double lon2 = GetLongitude(orbit, u_90, fi2, pls2) - Constants.Omega * quart1;// Period / 4.0;

        int ch23 = 0;

        if (lon1 < 0.0 && lon2 < 0.0)
            ch23++;
        if (lon1 > 0.0 && lon2 < 0.0)
            if (di1_90 < di2_90)
                ch23++;
        if (lon1 < 0.0 && lon2 > 0.0)
            if (di1_90 > di2_90)
                ch23++;

        lon1 = SpaceMath.TWOPI + GetLongitude(orbit, u_270, fi1, pls1) - Constants.Omega * quart3;// 3.0 * Period / 4.0;
        lon2 = SpaceMath.TWOPI + GetLongitude(orbit, u_270, fi2, pls2) - Constants.Omega * quart3;// 3.0 * Period / 4.0;

        int ch4 = ch23;

        if (lon1 > SpaceMath.TWOPI && lon2 > SpaceMath.TWOPI)
            ch4++;
        if (lon1 > SpaceMath.TWOPI && lon2 < SpaceMath.TWOPI)
            if (di1_270 > di2_270)
                ch4++;
        if (lon1 < SpaceMath.TWOPI && lon2 > SpaceMath.TWOPI)
            if (di1_270 < di2_270)
                ch4++;

        int mdf = 0;

        if (ch4 == 2)
            mdf = -1;
        if (ch4 == 1)
            mdf = 0;
        if (ch4 == 0)
            mdf = 1;

        int pmdf;

        if (orbit.InclinationNormal + fi1 < SpaceMath.HALFPI
            && orbit.InclinationNormal + fi2 < SpaceMath.HALFPI)
            pmdf = 1;
        else
            pmdf = 0;

        Offset = mdf;
        Quart23 = ch23;
        Quart4 = ch4;
        Polis = pmdf;
    }

    private static double GetLongitude(Orbit orbit, double u, double fi, int pls)
    {
        double uTr = Math.Acos(Math.Cos(fi) * Math.Cos(u));
        double iTr = orbit.Inclination - Math.Atan2(Math.Tan(fi), Math.Sin(u)) * pls;
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));

        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
        {
            asinlon = SpaceMath.Sign(asinlon);
        }

        double lon = Math.Asin(asinlon);

        return lon;
    }

    public int Offset { get; }  // смещение

    public int Quart23 { get; }

    public int Quart4 { get; }

    public int Polis { get; }
}
