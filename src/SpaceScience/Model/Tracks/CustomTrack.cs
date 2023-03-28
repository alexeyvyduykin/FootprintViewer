namespace SpaceScience.Model;

public class CustomTrack //: Track
{
    private readonly Orbit _orbit;
    protected int _dir;

    public CustomTrack(Orbit orbit, double alpha1DEG, TrackPointDirection direction)// : base(orbit)
    {
        _orbit = orbit;

        Alpha1 = alpha1DEG * SpaceMath.DegreesToRadians;

        Direction = direction;

        _dir = direction switch
        {
            TrackPointDirection.None => 0,
            TrackPointDirection.Left => -1,
            TrackPointDirection.Right => 1,
            _ => 0,
        };
    }

    public Orbit Orbit => _orbit;

    public double Alpha1 { get; }

    public double Alpha2 { get; } = 0.0;

    public TrackPointDirection Direction { get; }

    public bool PolisMod(double lat, ref double polis_mod)
    {
        double t_polis;
        int err;
        if (lat >= 0.0)
        { t_polis = Orbit.TimeHalfPi(); err = 1; }
        else
        { t_polis = 3.0 * Orbit.TimeHalfPi(); err = -1; }

        //double per = 3.0 * Orbit.Period / 4.0;

        double fi = CentralAngleFromT(t_polis + Orbit.ArgumentOfPerigee * Orbit.Period / SpaceMath.TWOPI);
        double i = Orbit.Inclination - fi * _dir;

        //double i_deg = Orbit.Inclination * ScienceMath.RadiansToDegrees;
        //double fi_deg = fi * ScienceMath.RadiansToDegrees;

        if (i > SpaceMath.HALFPI)
            i = Math.PI - i;
        if (i <= Math.Abs(lat))
        {
            polis_mod = Orbit.InclinationNormal + fi * _dir * err;
            return true;
        }
        return false;
    }

    public double CentralAngleFromT(double t)
    {
        double u = Orbit.Anomalia(t) + Orbit.ArgumentOfPerigee;
        double semi_axis = Orbit.Semiaxis(u);
        return SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(Alpha1) / Constants.Re) - Alpha1;
    }

    public (double lonRad, double latRad) ContinuousTrack(double node, double t, double tPastAN, int quart)
    {
        double v = Orbit.Anomalia(t, tPastAN);
        double u = v + Orbit.ArgumentOfPerigee;

        double semi_axis = Orbit.Semiaxis(u);
        double angle = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(Alpha1) / Constants.Re) - Alpha1;
        double uTr = angle == 0.0 ? u : Math.Acos(Math.Cos(angle) * Math.Cos(u));
        double iTr = Orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _dir;
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
            asinlon = SpaceMath.Sign(asinlon);

        double lon = Math.Asin(asinlon);

        //if( quart == 1 )lon = lon;
        if (quart == 2 || quart == 3)
            lon = Math.PI - lon;// - factor.ch23 * SpaceMath.TWOPI;
        else if (quart == 4)
            lon = SpaceMath.TWOPI + lon;// - factor.ch4 * SpaceMath.TWOPI;

        lon = Orbit.LonAscnNode + lon - Constants.Omega * (t + tPastAN) + node * SpaceMath.TWOPI;// * factor.mdf;
        return (lon, lat);
    }

    public (double lonRad, double latRad) TrackPoint(double u)
    {
        double uTr, iTr;
        if (Alpha1 == 0.0)
        {
            uTr = u;
            iTr = Orbit.Inclination;
        }
        else
        {
            var angle = CentralAngleFromU(u);
            uTr = Math.Acos(Math.Cos(angle) * Math.Cos(u));
            iTr = Orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _dir;
        }
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);
        if (Math.Abs(asinlon) > 1.0)
            asinlon = Math.Sign(asinlon);
        double lon = 0.0;
        if (u >= 0.0 && u < SpaceMath.HALFPI)
            lon = Math.Asin(asinlon);
        else if (u >= SpaceMath.HALFPI && u < 3.0 * SpaceMath.HALFPI)
            lon = Math.PI - Math.Asin(asinlon);
        else if (u >= 3.0 * SpaceMath.HALFPI && u < SpaceMath.TWOPI)

            lon = SpaceMath.TWOPI + Math.Asin(asinlon);
        return (lon, lat);
    }

    private double CentralAngleFromU(double u)
    {
        double semiAxis, alphaGround, angle;
        semiAxis = Orbit.Semiaxis(u);
        angle = SpaceMath.HALFPI - Math.Acos(semiAxis * Math.Sin(Alpha1) / Constants.Re) - Alpha1;
        if (Alpha2 != 0.0)
        {
            alphaGround = Math.Atan(Math.Tan(Alpha2) * Math.Sin(Alpha1));
            angle = Math.Asin(Math.Sin(angle) / Math.Cos(alphaGround));
        }
        return angle;
    }
}
