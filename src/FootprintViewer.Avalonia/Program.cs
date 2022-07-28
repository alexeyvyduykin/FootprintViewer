using Avalonia;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;

namespace FootprintViewer.Avalonia
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            AppMode mode = AppMode.Release;

            if (args.Length != 0)
            {
                if (Enum.TryParse(typeof(AppMode), args[0], true, out var res) == true)
                {
                    mode = (AppMode)res!;
                }
            }

            RegisterBootstrapper(mode);

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        private static void RegisterBootstrapper(AppMode mode)
        {
            // I only want to hear about errors
            var logger = new ConsoleLogger() { Level = Splat.LogLevel.Error };
            Locator.CurrentMutable.RegisterConstant(logger, typeof(ILogger));

            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, mode);

            // IViewFor
            //Locator.CurrentMutable.Register(() => new Views.MainView(), typeof(IViewFor<MainViewModel>));
            //Locator.CurrentMutable.Register(() => new Views.WorldMapSelectorView(), typeof(IViewFor<WorldMapSelector>));

            // lists
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.GroundStationInfoListView(), typeof(IViewFor<GroundStationViewerList>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.GroundTargetInfoListView(), typeof(IViewFor<GroundTargetViewerList>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.SatelliteInfoListView(), typeof(IViewFor<SatelliteViewerList>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.FootprintPreviewListView(), typeof(IViewFor<FootprintPreviewViewerList>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ListTemplates.UserGeometryInfoListView(), typeof(IViewFor<UserGeometryViewerList>));

            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.PreviewMainContentView(), typeof(IViewFor<PreviewMainContent>));

            // items
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintItemView(), typeof(IViewFor<FootprintViewModel>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.FootprintPreviewItemView(), typeof(IViewFor<FootprintPreviewViewModel>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundTargetItemView(), typeof(IViewFor<GroundTargetViewModel>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.SatelliteItemView(), typeof(IViewFor<SatelliteViewModel>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.UserGeometryItemView(), typeof(IViewFor<UserGeometryViewModel>));
            Locator.CurrentMutable.Register(() => new Views.SidePanelTabs.ItemTemplates.GroundStationItemView(), typeof(IViewFor<GroundStationViewModel>));
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}
