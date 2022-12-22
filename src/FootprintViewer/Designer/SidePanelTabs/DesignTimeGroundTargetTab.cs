using FootprintViewer.ViewModels.SidePanel.Tabs;
using Splat;

namespace FootprintViewer.Designer;

public class DesignTimeGroundTargetTab : GroundTargetTabViewModel
{
    private static readonly IReadonlyDependencyResolver _dependencyResolver = new DesignTimeData();

    public DesignTimeGroundTargetTab() : base(_dependencyResolver)
    {
        IsActive = true;
    }
}
