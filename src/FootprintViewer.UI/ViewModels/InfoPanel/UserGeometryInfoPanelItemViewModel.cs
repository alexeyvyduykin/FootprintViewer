using FootprintViewer.UI.ViewModels.SidePanel.Items;

namespace FootprintViewer.UI.ViewModels.InfoPanel;

public sealed class UserGeometryInfoPanelItemViewModel : InfoPanelItemViewModel
{
    public static UserGeometryInfoPanelItemViewModel Create(UserGeometryViewModel geometry) => new(geometry);

    public UserGeometryInfoPanelItemViewModel()
    {
        Key = "UserGeometry";
    }

    private UserGeometryInfoPanelItemViewModel(UserGeometryViewModel geometry) : this()
    {
        Text = geometry.Name;

        TypeInfo = geometry.Type.ToString();
    }

    public string? TypeInfo { get; set; }
}