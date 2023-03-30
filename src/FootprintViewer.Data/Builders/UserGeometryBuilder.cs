using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.Builders;

public static class UserGeometryBuilder
{
    private static readonly Random _random = new();

    public static UserGeometry CreateRandom()
    {
        return new UserGeometry()
        {
            Name = $"UserGeometry{_random.Next(1, 101):000}",
            Type = (UserGeometryType)_random.Next(0, 4),
        };
    }
}
