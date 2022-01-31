using FootprintViewer.ViewModels;
using Splat;
using System.Collections.ObjectModel;

namespace FootprintViewer.Designer
{
    public class DesignerContext
    {
        public static MainViewModel? MainViewModel { get; private set; }

        public static void InitializeContext(IReadonlyDependencyResolver dependencyResolver)
        {
            // Map

            var map = dependencyResolver.GetService<Mapsui.Map>();

            // MainViewModel

            MainViewModel = dependencyResolver.GetExistingService<MainViewModel>();

            //     MainViewModel.SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));
        }
    }

    public class ObservableGroundTargetCollection : ObservableCollection<GroundTargetInfo> { }
}
