using CommunityToolkit.Mvvm.DependencyInjection;

namespace FootprintViewer.UI;

public static class Services
{
    public static string DataDir { get; private set; } = null!;

    public static string MapSnapshotDir { get; private set; } = null!;

    public static Config Config { get; private set; } = new();

    public static void Initialize(Global global)
    {
        DataDir = global.DataDir;

        MapSnapshotDir = global.MapSnapshotDir;

        Config = global.Config;
    }

    public static Ioc Locator => Ioc.Default;
}
