using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeSidePanel : SidePanel
    {
        public DesignTimeSidePanel() : base()
        {
            var data = new DesignTimeData();

            var tabs = new SidePanelTab[]
            {
                new SceneSearch(data),
                new SatelliteViewer(data),
                new GroundTargetViewer(data),
                new FootprintObserver(data),
                new UserGeometryViewer(data),
            };

            Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }
}
