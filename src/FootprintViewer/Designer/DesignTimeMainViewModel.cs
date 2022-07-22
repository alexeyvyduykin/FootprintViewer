using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeMainViewModel : MainViewModel
    {
        public static readonly DesignTimeData _designTimeData = new();

        public DesignTimeMainViewModel() : base(_designTimeData)
        {
            var tabs = new SidePanelTab[]
            {
                new FootprintPreviewTab(_designTimeData),
                new SatelliteTab(_designTimeData),
                new GroundStationTab(_designTimeData),
                new GroundTargetTab(_designTimeData),
                new FootprintTab(_designTimeData),
                new UserGeometryTab(_designTimeData),
            };

            SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }
}
