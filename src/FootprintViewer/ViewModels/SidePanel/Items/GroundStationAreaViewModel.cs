using Mapsui.Styles;

namespace FootprintViewer.ViewModels.SidePanel.Items;

public class GroundStationAreaViewModel : ViewModelBase
{
    public Color Color { get; set; } = new();

    public double Angle { get; set; }
}
