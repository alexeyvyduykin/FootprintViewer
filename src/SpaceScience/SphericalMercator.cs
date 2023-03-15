namespace SpaceScience;

public class SphericalMercator2
{
    private const double radius = 6378137.0;
    private const double degreesToRadians = Math.PI / 180.0;

    public static (double x, double y) FromLonLat(double lon, double lat)
    {
        double num = degreesToRadians * lon;
        double num2 = degreesToRadians * lat;
        double item = radius * num;
        double item2 = radius * Math.Log(Math.Tan(Math.PI / 4.0 + num2 * 0.5));
        return (item, item2);
    }

    public static (double lon, double lat) ToLonLat(double x, double y)
    {
        double d = Math.Exp((0.0 - y) / radius);
        double num = SpaceMath.HALFPI - 2.0 * Math.Atan(d);
        double item = x / radius / degreesToRadians;
        double item2 = num / degreesToRadians;
        return (item, item2);
    }
}
