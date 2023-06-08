using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using System.Reactive.Linq;

namespace FootprintViewer.Data.Builders;

public static class GroundStationBuilder
{
    private static Random _random = new();

    private static async Task<T> Start<T>(Func<T> func)
        => await Observable.Start(func);

    public static async Task<IList<GroundStation>> CreateDefaultAsync()
        => await Start(() => CreateDefault());

    public static GroundStation CreateRandom()
    {
        return new GroundStation()
        {
            Name = $"London",
            Center = new Point(-0.118092, 51.509865),
            Angles = new double[] { 12, 18, 22, 26, 30 },
        };
    }

    public static IList<GroundStation> CreateDefault()
    {
        return new List<GroundStation>()
        {
            new GroundStation() { Name = "Tokyo",       Center = new Point(139.41,  35.41), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "São Paulo",   Center = new Point(-46.38, -23.33), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Mexico City", Center = new Point(-99.80,  19.26), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "New York",    Center = new Point(-74.00,  40.42), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Paris",       Center = new Point(  2.21,  48.51), Angles = new [] { 0.0, 6, 10, 11 } },
            new GroundStation() { Name = "Seoul",       Center = new Point(126.59,  37.33), Angles = new [] { 0.0, 6, 10, 11 } },
        };
    }
}
