using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;

namespace FootprintViewer.Avalonia
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            // I only want to hear about errors
            var logger = new ConsoleLogger() { Level = Splat.LogLevel.Error };
            Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));

            // IViewFor
            Locator.CurrentMutable.Register(() => new Views.MainView(), typeof(IViewFor<MainViewModel>));
            //Locator.CurrentMutable.Register(() => new Views.WorldMapSelectorView(), typeof(IViewFor<WorldMapSelector>));

            // lists
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.FootprintInfoListView(), typeof(IViewFor<ViewerList<FootprintInfo>>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.GroundStationInfoListView(), typeof(IViewFor<ViewerList<GroundStationInfo>>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.GroundTargetInfoListView(), typeof(IViewFor<ViewerList<GroundTargetInfo>>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.SatelliteInfoListView(), typeof(IViewFor<ViewerList<SatelliteInfo>>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.FootprintPreviewListView(), typeof(IViewFor<ViewerList<FootprintPreview>>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.UserGeometryInfoListView(), typeof(IViewFor<ViewerList<UserGeometryInfo>>));

            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.PreviewMainContentView(), typeof(IViewFor<PreviewMainContent>));

            // filters
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.FilterTemplates.FootprintInfoFilterView(), typeof(IViewFor<FootprintObserverFilter>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.FilterTemplates.FootprintPreviewFilterView(), typeof(IViewFor<SceneSearchFilter>));

            // items
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintInfoView(), typeof(IViewFor<FootprintInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintPreviewView(), typeof(IViewFor<FootprintPreview>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundTargetInfoView(), typeof(IViewFor<GroundTargetInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.SatelliteInfoView(), typeof(IViewFor<SatelliteInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.UserGeometryInfoView(), typeof(IViewFor<UserGeometryInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundStationInfoView(), typeof(IViewFor<GroundStationInfo>));

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
