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
                new SceneSearch(_designTimeData),
                new SatelliteViewer(_designTimeData),
                new GroundTargetViewer(_designTimeData),
                new FootprintObserver(_designTimeData),
                new UserGeometryViewer(_designTimeData),
            };

            SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }
}
