namespace SpaceScience.Model;

public class FactorTrack : CustomTrack
{
    private readonly FactorShiftTrack _factor;

    public FactorTrack(CustomTrack track, FactorShiftTrack factor) : base(track.Orbit, track.Alpha1 * SpaceMath.RadiansToDegrees, track.Direction)
    {
        _factor = factor;
    }

    public override Geo2D ContinuousTrack(double node, double t, double tPastAN, int quart)
    {
        double v = Orbit.Anomalia(t, tPastAN);
        double u = v + Orbit.ArgumentOfPerigee;

        double semi_axis = Orbit.Semiaxis(u);
        double angle = SpaceMath.HALFPI - Math.Acos(semi_axis * Math.Sin(Alpha1) / Constants.Re) - Alpha1;
        double uTr = Math.Acos(Math.Cos(angle) * Math.Cos(u));
        double iTr = Orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _dir;
        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
            asinlon = SpaceMath.Sign(asinlon);

        double lon = Math.Asin(asinlon);

        //if( quart == 1 )lon = lon;
        if (quart == 2 || quart == 3)
            lon = Math.PI - lon - _factor.Quart23 * SpaceMath.TWOPI;
        else if (quart == 4)
            lon = SpaceMath.TWOPI + lon - _factor.Quart4 * SpaceMath.TWOPI;

        lon = Orbit.LonAscnNode + lon - Constants.Omega * (t + tPastAN) + node * SpaceMath.TWOPI * _factor.Offset;
        return new Geo2D(lon, lat, GeoCoordTypes.Radians);
    }
}
