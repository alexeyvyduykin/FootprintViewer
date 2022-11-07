using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeMainViewModel : MainViewModel
    {
        public static readonly DesignTimeData _designTimeData = new();

        public DesignTimeMainViewModel() : base(_designTimeData)
        {
            var tabs = new SidePanelTabViewModel[]
            {
                new FootprintPreviewTabViewModel(_designTimeData),
                new SatelliteTabViewModel(_designTimeData),
                new GroundStationTabViewModel(_designTimeData),
                new GroundTargetTabViewModel(_designTimeData),
                new FootprintTabViewModel(_designTimeData),
                new UserGeometryTabViewModel(_designTimeData),
            };

            SidePanel.Tabs.AddRange(new List<SidePanelTabViewModel>(tabs));
        }
    }
}
