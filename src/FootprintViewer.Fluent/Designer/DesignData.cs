using FootprintViewer.Data.Builders;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;
using FootprintViewer.Fluent.ViewModels.InfoPanel;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Fluent.ViewModels.Settings;
using FootprintViewer.Fluent.ViewModels.SidePanel;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;
using FootprintViewer.Fluent.ViewModels.Timelines;
using FootprintViewer.Fluent.ViewModels.Tips;
using FootprintViewer.Fluent.ViewModels.ToolBar;
using Mapsui;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FootprintViewer.Fluent.Designer;

public static class DesignData
{
    private static readonly DesignDataDependencyResolver _resolver = new();

    // Side panel filters
    public static FootprintPreviewTabFilterViewModel FootprintPreviewFilter => new();

    public static FootprintTabFilterViewModel FootprintFilter => new(_resolver);

    public static GroundTargetTabFilterViewModel GroundTargetFilter => new();

    public static ToolBarViewModel ToolBar => new(_resolver);

    public static LayerContainerViewModel LayerContainer => new(_resolver);

    public static CustomTipViewModel CustomTip => CustomTipViewModel.HoverCreating(TipTarget.Rectangle, 34545.432);

    // Side panel tabs
    public static FootprintPreviewTabViewModel FootprintPreviewTab => new() { IsActive = true };

    public static FootprintTabViewModel FootprintTab => new(_resolver)
    {
        SearchString = "footprint",
        IsActive = true,
    };

    public static GroundStationTabViewModel GroundStationTab => new(_resolver) { IsActive = true };

    public static GroundTargetTabViewModel GroundTargetTab => new(_resolver) { IsActive = true };

    public static SatelliteTabViewModel SatelliteTab => new(_resolver) { IsActive = true };

    public static UserGeometryTabViewModel UserGeometryTab => new(_resolver) { IsActive = true };

    public static PlannedScheduleTabViewModel PlannedScheduleTab => new(_resolver) { IsActive = true };

    public static FootprintPreviewViewModel FootprintPreview => new(FootprintPreviewBuilder.CreateRandom());

    public static FootprintViewModel Footprint => new(FootprintBuilder.CreateRandom()) { IsShowInfo = true };

    public static TaskResultViewModel TaskResult =>
        new(TaskResultBuilder.CreateObservation("ObservationTask0063", FootprintBuilder.CreateRandom()));

    public static GroundStationViewModel GroundStation => new(GroundStationBuilder.CreateRandom()) { IsShow = true };

    public static GroundTargetViewModel GroundTarget => new(GroundTargetBuilder.CreateRandom());

    public static SatelliteViewModel Satellite => new(SatelliteBuilder.CreateRandom()) { IsShow = true, IsShowInfo = true };

    public static UserGeometryViewModel UserGeometry => new(UserGeometryBuilder.CreateRandom());

    public static TimelinesViewModel Timelines => new();

    public static TimelinesOldViewModel TimelinesOld => new();

    public static FootprintClickInfoPanel FootprintClickInfoPanel => new(new FootprintViewModel(FootprintBuilder.CreateRandom()));

    public static GroundTargetClickInfoPanel GroundTargetClickInfoPanel => new(new GroundTargetViewModel(GroundTargetBuilder.CreateRandom()));

    public static RouteInfoPanel RouteInfoPanel => new() { Text = "Description" };

    public static AOIInfoPanel AOIInfoPanel => new() { Text = "Description" };

    public static UserGeometryClickInfoPanel UserGeometryClickInfoPanel => new(new UserGeometryViewModel(UserGeometryBuilder.CreateRandom()));

    public static InfoPanelViewModel InfoPanel => CreateInfoPanel();

    // Dialogs
    public static RoutableViewModel ContentArea => new ContentAreaDesign() { IsActive = true };

    public static AddPlannedSchedulePageViewModel AddPlannedSchedulePage => new() { IsActive = true };

    public static DemoPageViewModel DemoPage => new() { IsActive = true };

    public static ConnectDatabasePageViewModel ConnectDatabasePage => new() { IsActive = true };

    public static ImportFilePageViewModel ImportFilePage => new(string.Empty) { IsActive = true };

    private static InfoPanelViewModel CreateInfoPanel()
    {
        var panel = new InfoPanelViewModel();

        panel.Show(RouteInfoPanel);
        panel.Show(AOIInfoPanel);
        panel.Show(FootprintClickInfoPanel);
        panel.Show(GroundTargetClickInfoPanel);
        panel.Show(UserGeometryClickInfoPanel);

        return panel;
    }

    public static SettingsViewModel Settings => new(_resolver) { IsActive = true };

    // Tool bar
    public static MapToolsViewModel MapTools => new(_resolver);

    private static string GetFullPathToAssets()
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var path = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));
        return path;
    }

    public static SidePanelViewModel SidePanel => CreateSidePanel(_resolver);

    public static ScaleMapBar ScaleMapBar => CreateScaleMapBar();

    public static MainViewModel MainViewModel => new(_resolver);// CreateMainViewModel();

    private static ScaleMapBar CreateScaleMapBar()
    {
        // ScaleMapBar
        var scaleMapBar = new ScaleMapBar();

        var viewport = new Viewport();
        viewport.SetSize(400, 200);
        viewport.SetResolution(2);

        scaleMapBar.ChangedPosition(new MPoint(50, 20));
        scaleMapBar.ChangedViewport(viewport);

        return scaleMapBar;
    }

    private static SidePanelViewModel CreateSidePanel(DesignDataDependencyResolver resolver)
    {
        var sidePanel = new SidePanelViewModel();

        var tabs = new SidePanelTabViewModel[]
        {
            new SatelliteTabViewModel(resolver),
            new GroundStationTabViewModel(resolver),
            new GroundTargetTabViewModel(resolver),
            new FootprintTabViewModel(resolver),
          //  new UserGeometryTabViewModel(resolver),          
            new PlannedScheduleTabViewModel(resolver)
        };

        var actiontabs = new SidePanelActionTabViewModel[]
        {
            new(nameof(AddPlannedSchedulePageViewModel)),
            new(nameof(SettingsViewModel)),
        };

        sidePanel.Tabs.AddRange(tabs);
        sidePanel.ActionTabs.AddRange(actiontabs);

        return sidePanel;
    }

    private static MainViewModel CreateMainViewModel()
    {
        var mainViewModel = new MainViewModel(_resolver);

        var tabs = new SidePanelTabViewModel[]
        {
            //new FootprintPreviewTabViewModel(),
            new SatelliteTabViewModel(),
            new GroundStationTabViewModel(),
            new GroundTargetTabViewModel(),
            new FootprintTabViewModel(),
            new UserGeometryTabViewModel(),
        };

        // mainViewModel.SidePanel.Tabs.Clear();
        // mainViewModel.SidePanel.Tabs.AddRange(tabs);

        return mainViewModel;
    }
}

public class SelectRecordPageDesignViewModel : RoutableViewModel
{
    public SelectRecordPageDesignViewModel()
    {
        EnableBack = true;
        EnableCancel = true;

        Items = new()
        {
            new PlannedScheduleItemViewModel("PlannedSchedule1", new DateTime(2023, 4, 20, 12, 32, 43)),
            new PlannedScheduleItemViewModel("PlannedSchedule2", new DateTime(2023, 3, 23, 22, 54, 11)),
            new PlannedScheduleItemViewModel("PlannedSchedule3", new DateTime(2023, 4, 25, 03, 02, 54)),
            new PlannedScheduleItemViewModel("PlannedSchedule4", new DateTime(2023, 4, 21, 11, 11, 44)),
            new PlannedScheduleItemViewModel("PlannedSchedule5", new DateTime(2023, 4, 18, 19, 22, 42)),
        };

        SelectedItem = Items.FirstOrDefault();

        IsActive = true;
    }

    public override string Title { get => "Select planned schedule"; protected set { } }

    public List<PlannedScheduleItemViewModel> Items { get; set; }

    public PlannedScheduleItemViewModel? SelectedItem { get; set; }
}

public class ContentAreaDesign : RoutableViewModel
{
    public ContentAreaDesign()
    {
        EnableBack = true;
        EnableCancel = true;
        IsActive = true;

        SkipCommand = ReactiveCommand.Create(() => { });
        NextCommand = ReactiveCommand.Create(() => { });
    }

    public override string Title { get => "This is a title"; protected set { } }
}