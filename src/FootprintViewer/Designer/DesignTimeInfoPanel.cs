using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeInfoPanel : InfoPanel
    {
        public DesignTimeInfoPanel() : base()
        {
            var RouteInfoPanel = new RouteInfoPanel()
            {
                Text = "Description",
            };

            var AoiInfoPanel = new AOIInfoPanel()
            {
                Text = "Description",
            };

            var footprintClickInfo = new FootprintClickInfoPanel(new FootprintInfo(DesignTimeFootprintInfo.BuildModel()));

            Show(RouteInfoPanel);
            Show(AoiInfoPanel);
            Show(footprintClickInfo);
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
}
