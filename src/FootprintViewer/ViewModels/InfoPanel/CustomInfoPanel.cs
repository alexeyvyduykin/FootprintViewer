using FootprintViewer.ViewModels.SidePanel.Items;

namespace FootprintViewer.ViewModels;

public abstract class CustomInfoPanel : InfoPanelItem, ISelectorItem
{
    public abstract string GetKey();

    public string? Text { get; set; }
}

public class AOIInfoPanel : CustomInfoPanel
{
    public override string GetKey() => "AOI";
}

public class RouteInfoPanel : CustomInfoPanel
{
    public override string GetKey() => "Route";
}

public class FootprintClickInfoPanel : CustomInfoPanel
{
    private readonly FootprintViewModel _footprintViewModel;

    public FootprintClickInfoPanel(FootprintViewModel footprintViewModel)
    {
        _footprintViewModel = footprintViewModel;
    }

    public FootprintViewModel FootprintViewModel => _footprintViewModel;

    public override string GetKey() => "FootprintClick";
}

public class GroundTargetClickInfoPanel : CustomInfoPanel
{
    private readonly GroundTargetViewModel _groundTargetViewModel;

    public GroundTargetClickInfoPanel(GroundTargetViewModel groundTargetViewModel)
    {
        _groundTargetViewModel = groundTargetViewModel;
    }

    public GroundTargetViewModel GroundTargetViewModel => _groundTargetViewModel;

    public override string GetKey() => "GroundTargetClick";
}

public class UserGeometryClickInfoPanel : CustomInfoPanel
{
    private readonly UserGeometryViewModel _userGeometryViewModel;

    public UserGeometryClickInfoPanel(UserGeometryViewModel userGeometryViewModel)
    {
        _userGeometryViewModel = userGeometryViewModel;
    }

    public UserGeometryViewModel UserGeometryViewModel => _userGeometryViewModel;

    public override string GetKey() => "UserGeometryClick";
}
