using FootprintViewer.ViewModels;
using Splat;
using System.Collections.Generic;

namespace FootprintViewer.Designer
{
    public class DesignTimeMainViewModel : MainViewModel
    {
        public static IReadonlyDependencyResolver _data = new DesignTimeData();

        public DesignTimeMainViewModel() : base(_data)
        {
            var tabs = new SidePanelTab[]
            {
                new SceneSearch(_data),
                new SatelliteViewer(_data),
                new GroundTargetViewer(_data),
                new FootprintObserver(_data), 
                new UserGeometryViewer(_data),
            };

            SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }
}
