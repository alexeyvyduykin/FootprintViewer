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

    public class DesignTimeInfoPanelItem : RouteInfoPanel
    {
        public DesignTimeInfoPanelItem() : base()
        {
            Text = "Description";
        }
    }

    public class DesignTimeFootprintClickInfoPanelItem : FootprintClickInfoPanel
    {
        public DesignTimeFootprintClickInfoPanelItem() : base(new FootprintViewModel(DesignTimeFootprintViewModel.BuildModel()))
        {

        }
    }

    public class DesignTimeGroundTargetClickInfoPanelItem : GroundTargetClickInfoPanel
    {
        public DesignTimeGroundTargetClickInfoPanelItem() : base(new GroundTargetViewModel(DesignTimeGroundTargetViewModel.BuildModel()))
        {

        }
    }

    public class DesignTimeUserGeometryClickInfoPanelItem : UserGeometryClickInfoPanel
    {
        public DesignTimeUserGeometryClickInfoPanelItem() : base(new UserGeometryViewModel(DesignTimeUserGeometryViewModel.BuildModel()))
        {

        }
    }
}
