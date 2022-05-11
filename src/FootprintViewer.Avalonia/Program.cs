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

            //Locator.CurrentMutable.Register(() => new Views.TipView(), typeof(IViewFor<Tip>));

            Locator.CurrentMutable.Register(() => new Views.ToolBar.CustomToolBarView(), typeof(IViewFor<CustomToolBar>));

            //Locator.CurrentMutable.Register(() => new Views.WorldMapSelectorView(), typeof(IViewFor<WorldMapSelector>));

            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.PreviewMainContentView(), typeof(IViewFor<PreviewMainContent>));
            //Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.SceneSearchFilterView(), typeof(IViewFor<SceneSearchFilter>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.FootprintObserverListView(), typeof(IViewFor<FootprintObserverList>));
            //Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.FootprintObserverFilterView(), typeof(IViewFor<FootprintObserverFilter>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.GroundTargetViewerListView(), typeof(IViewFor<GroundTargetViewerList>));

            // TODO: this item templates freeze app
            //Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintInfoView(), typeof(IViewFor<FootprintInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintPreviewView(), typeof(IViewFor<FootprintPreview>));
            //Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundTargetInfoView(), typeof(IViewFor<GroundTargetInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.SatelliteInfoView(), typeof(IViewFor<SatelliteInfo>));
            //Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.UserGeometryInfoView(), typeof(IViewFor<UserGeometryInfo>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundStationInfoView(), typeof(IViewFor<GroundStationInfo>));

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
