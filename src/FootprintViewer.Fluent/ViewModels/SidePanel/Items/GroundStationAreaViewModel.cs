using Mapsui.Styles;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Items;

public class GroundStationAreaViewModel : ViewModelBase
{
    public Color Color { get; set; } = new();

    public double Angle { get; set; }
}
