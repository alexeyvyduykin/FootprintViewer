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

            Show(RouteInfoPanel);
            Show(AoiInfoPanel);
        }
    }

    public class DesignTimeInfoPanelItem : CustomInfoPanel
    {
        public DesignTimeInfoPanelItem() : base()
        {
            Text = "Description";
        }

        public override string Title => "Route";
    }
}
