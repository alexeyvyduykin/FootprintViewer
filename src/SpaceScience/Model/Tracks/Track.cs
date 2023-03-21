namespace SpaceScience.Model;

public class Track
{
    public Track(Orbit orbit)
    {
        Orbit = orbit;
    }

    /// <summary>
    /// lon(rad) => (-PI; +PI)
    /// </summary>
    /// <param name="tnorm"></param>
    /// <returns></returns>
    public Geo2D Position1(double tnorm)
    {
        double u = SpaceMath.WrapAngle(Orbit.Anomalia(tnorm) + Orbit.ArgumentOfPerigee);
        double lat = Math.Asin(Math.Sin(u) * Math.Sin(Orbit.Inclination));
        double asinlon = Math.Tan(lat) / Math.Tan(Orbit.Inclination);
        if (Math.Abs(asinlon) > 1.0)
            asinlon = SpaceMath.Sign(asinlon);
        double lon = Math.Asin(asinlon);
        if (u <= SpaceMath.HALFPI && u >= 0)
            lon = Math.Asin(asinlon);
        if (u > SpaceMath.HALFPI && u <= 3 * Math.PI / 2)
            lon = Math.PI - lon;
        if (u > 3 * Math.PI / 2 && u <= SpaceMath.TWOPI)
            lon = SpaceMath.TWOPI + lon;
        lon = Orbit.LonAscnNode + lon - Constants.Omega * tnorm;
        while (lon > Math.PI)
            lon -= SpaceMath.TWOPI;
        while (lon < -Math.PI)
            lon += SpaceMath.TWOPI;
        return new Geo2D(lon, lat, GeoCoordTypes.Radians);
    }

    public virtual Geo2D ContinuousTrack(double node, double t, double tPastAN, int quart)
    {
        double v = Orbit.Anomalia(t, tPastAN);
        double u = v + Orbit.ArgumentOfPerigee;

        double lat = Math.Asin(Math.Sin(u) * Math.Sin(Orbit.Inclination));
        double asinlon = Math.Tan(lat) / Math.Tan(Orbit.Inclination);

        if (Math.Abs(asinlon) > 1.0)
            asinlon = SpaceMath.Sign(asinlon);

        double lon = Math.Asin(asinlon);

        //if( quart == 1 )lon = lon;
        if (quart == 2 || quart == 3)
            lon = Math.PI - lon;// - factor.ch23 * SpaceMath.TWOPI;
        else if (quart == 4)
            lon = SpaceMath.TWOPI + lon;// - factor.ch4 * SpaceMath.TWOPI;

        lon = Orbit.LonAscnNode + lon - Constants.Omega * (t + tPastAN) + node * SpaceMath.TWOPI;// * factor.mdf;
        return new Geo2D(lon, lat);
    }

    public virtual Geo2D TrackPoint(double u)
    {
        double lat = Math.Asin(Math.Sin(u) * Math.Sin(Orbit.Inclination));
        double asinlon = Math.Tan(lat) / Math.Tan(Orbit.Inclination);
        if (Math.Abs(asinlon) > 1.0)
            asinlon = Math.Sign(asinlon);
        double lon = 0.0;
        if (u >= 0.0 && u < SpaceMath.HALFPI)
            lon = Math.Asin(asinlon);
        else if (u >= SpaceMath.HALFPI && u < 3.0 * SpaceMath.HALFPI)
            lon = Math.PI - Math.Asin(asinlon);
        else if (u >= 3.0 * SpaceMath.HALFPI && u < SpaceMath.TWOPI)
            lon = SpaceMath.TWOPI + Math.Asin(asinlon);
        return new Geo2D(lon, lat);
    }

    public Orbit Orbit { get; }
}
