namespace SpaceScience;

public static class SpaceConverters
{
    public static Geo2D From0To360(Geo2D point)
    {
        if (point.Type == GeoCoordTypes.Radians)
        {
            var lon = point.Lon;
            while (lon > 2.0 * Math.PI)
            {
                lon -= 2.0 * Math.PI;
            }
            while (lon < 0.0)
            {
                lon += 2.0 * Math.PI;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Radians);
        }
        else
        {
            var lon = point.Lon;
            while (lon > 360.0)
            {
                lon -= 360.0;
            }
            while (lon < 0.0)
            {
                lon += 360.0;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Degrees);
        }

    }

    public static Geo2D From180To180(Geo2D point)
    {
        if (point.Type == GeoCoordTypes.Radians)
        {
            var lon = point.Lon;
            while (lon > Math.PI)
            {
                lon -= 2.0 * Math.PI;
            }
            while (lon < -Math.PI)
            {
                lon += 2.0 * Math.PI;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Radians);
        }
        else
        {
            var lon = point.Lon;
            while (lon > 180.0)
            {
                lon -= 360.0;
            }
            while (lon < -180.0)
            {
                lon += 360.0;
            }

            return new Geo2D(lon, point.Lat, GeoCoordTypes.Degrees);
        }
    }
}
