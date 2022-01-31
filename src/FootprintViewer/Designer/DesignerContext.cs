using FootprintViewer.ViewModels;
using Splat;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FootprintViewer.Designer
{
    public class DesignerContext
    {
        public static SidePanel? SidePanel { get; private set; }

        public static ToolBar? ToolBar { get; private set; }

        public static MainViewModel? MainViewModel { get; private set; }

        public static void InitializeContext(IReadonlyDependencyResolver dependencyResolver)
        {
            // Map

            var map = dependencyResolver.GetService<Mapsui.Map>();

            // SidePanel

            var tabs = new SidePanelTab[]
            {

                new GroundTargetViewer(dependencyResolver)
                {
                    Name = "Test1",
                    Title = "Default test title1"
                },
                new GroundTargetViewer(dependencyResolver)
                {
                    Name = "Test2",
                    Title = "Default test title2"
                }
            };

            SidePanel = new SidePanel() { Tabs = new List<SidePanelTab>(tabs) };

            // ToolBar

            ToolBar = dependencyResolver.GetService<ToolBar>();

            // MainViewModel

            MainViewModel = dependencyResolver.GetExistingService<MainViewModel>();

            MainViewModel.SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }

    public class ObservableGroundTargetCollection : ObservableCollection<GroundTargetInfo> { }
}
