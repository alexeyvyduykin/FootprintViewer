using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeInfoPanel : InfoPanel
    {
        public DesignTimeInfoPanel() : base()
        {
            Show(new RouteInfoPanel() { Text = "Description" });
            Show(new AOIInfoPanel() { Text = "Description" });
            Show(new FootprintClickInfoPanel(new FootprintViewModel(DesignTimeFootprintViewModel.BuildModel())));
            Show(new GroundTargetClickInfoPanel(new GroundTargetViewModel(DesignTimeGroundTargetViewModel.BuildModel())));
            Show(new UserGeometryClickInfoPanel(new UserGeometryViewModel(DesignTimeUserGeometryViewModel.BuildModel())));
        }
    }
}
