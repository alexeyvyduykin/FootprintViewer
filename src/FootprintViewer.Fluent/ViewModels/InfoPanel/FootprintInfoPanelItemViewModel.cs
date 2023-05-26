using FootprintViewer.UI.ViewModels.SidePanel.Items;

namespace FootprintViewer.UI.ViewModels.InfoPanel;

public sealed class FootprintInfoPanelItemViewModel : InfoPanelItemViewModel
{
    public static FootprintInfoPanelItemViewModel Create(FootprintViewModel footprint) => new(footprint);

    public FootprintInfoPanelItemViewModel()
    {
        Key = "Footprint";
    }

    private FootprintInfoPanelItemViewModel(FootprintViewModel footprint) : this()
    {
        var coordinate = footprint.Center;
        var begin = footprint.Begin;
        var duration = footprint.Duration;
        var satelliteName = footprint.SatelliteName;
        var node = footprint.Node;
        var direction = footprint.Direction;

        SatelliteInfo = $"{satelliteName} ({node} - {direction})";

        TimeInfo = $"{begin: dd.MM.yyyy HH:mm:ss} ({duration} sec)";

        CenterInfo = $"{coordinate.X:0.00}° {coordinate.Y:0.00}°";

        Text = footprint.Name;
    }

    public string? CenterInfo { get; set; }

    public string? SatelliteInfo { get; set; }

    public string? TimeInfo { get; set; }
}
