using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeInfoPanel : InfoPanel
    {
        public DesignTimeInfoPanel() : base()
        {
            Show(new RouteInfoPanel() { Text = "Description" });
            Show(new AOIInfoPanel() { Text = "Description" });
            Show(new FootprintClickInfoPanel(new FootprintInfo(DesignTimeFootprintInfo.BuildModel())));
            Show(new GroundTargetClickInfoPanel(new GroundTargetInfo(DesignTimeGroundTargetInfo.BuildModel())));
            Show(new UserGeometryClickInfoPanel(new UserGeometryInfo(DesignTimeUserGeometryInfo.BuildModel())));
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
        public DesignTimeFootprintClickInfoPanelItem() : base(new FootprintInfo(DesignTimeFootprintInfo.BuildModel()))
        {

        }
    }

    public class DesignTimeGroundTargetClickInfoPanelItem : GroundTargetClickInfoPanel
    {
        public DesignTimeGroundTargetClickInfoPanelItem() : base(new GroundTargetInfo(DesignTimeGroundTargetInfo.BuildModel()))
        {

        }
    }

    public class DesignTimeUserGeometryClickInfoPanelItem : UserGeometryClickInfoPanel
    {
        public DesignTimeUserGeometryClickInfoPanelItem() : base(new UserGeometryInfo(DesignTimeUserGeometryInfo.BuildModel()))
        {

        }
    }
}
