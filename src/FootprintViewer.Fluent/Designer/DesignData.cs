using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;
using FootprintViewer.Fluent.ViewModels.InfoPanel;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Fluent.ViewModels.Settings;
using FootprintViewer.Fluent.ViewModels.Settings.SourceBuilders;
using FootprintViewer.Fluent.ViewModels.SidePanel;
using FootprintViewer.Fluent.ViewModels.SidePanel.Filters;
using FootprintViewer.Fluent.ViewModels.SidePanel.Items;
using FootprintViewer.Fluent.ViewModels.SidePanel.Tabs;
using FootprintViewer.Fluent.ViewModels.Timelines;
using FootprintViewer.Fluent.ViewModels.Tips;
using FootprintViewer.Fluent.ViewModels.ToolBar;
using Mapsui;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FootprintViewer.Fluent.Designer;

public static class DesignData
{
    private static readonly DesignDataDependencyResolver _resolver = new();

    public static FootprintPreviewTabFilterViewModel FootprintPreviewFilter => new();

    public static FootprintTabFilterViewModel FootprintFilter => new();

    public static GroundTargetTabFilterViewModel GroundTargetFilter => new();

    public static BottomPanel BottomPanel => new();

    public static SnapshotMaker SnapshotMaker => new();

    public static ToolBarViewModel ToolBar => new();

    public static LayerContainerViewModel LayerContainer => new();

    public static CustomTipViewModel CustomTip => CustomTipViewModel.BeginCreating(TipTarget.Rectangle, 34545.432);

    public static FootprintPreviewTabViewModel FootprintPreviewTab => new() { IsActive = true };

    public static FootprintTabViewModel FootprintTab => new()
    {
        SearchString = "footprint",
        IsActive = true,
    };

    public static GroundStationTabViewModel GroundStationTab => new() { IsActive = true };

    public static GroundTargetTabViewModel GroundTargetTab => new() { IsActive = true };

    public static SatelliteTabViewModel SatelliteTab => new() { IsActive = true };

    public static UserGeometryTabViewModel UserGeometryTab => new() { IsActive = true };

    public static PlannedScheduleTabViewModel PlannedScheduleTab => new() { IsActive = true };

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

    public static TableInfoViewModel TableInfo => TableInfoViewModel.Build(TableInfoType.UserGeometry);

    public static SettingsViewModel Settings => new(_resolver) { IsActive = true };

    public static DatabaseBuilderViewModel DatabaseBuilder => new(DbKeys.UserGeometries, new DbFactory())
    {
        Host = "localhost",
        Port = 5432,
        Database = "DataSettingsSampleDatabase1",
        Username = "postgres",
        Password = "user",
        IsActive = true
    };

    public static JsonBuilderViewModel JsonBuilder => new(DbKeys.UserGeometries)
    {
        Directory = GetFullPathToAssets(),
        IsActive = true
    };

    public static ConnectionViewModel Connection => CreateConnection();

    private static string GetFullPathToAssets()
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var path = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));
        return path;
    }

    private static ConnectionViewModel CreateConnection()
    {
        var connection = new ConnectionViewModel();

        var source = _resolver.GetService<IDataManager>()?.GetSources(DbKeys.UserGeometries.ToString())[0];

        if (source is not null)
        {
            connection.SourceContainers = new List<SourceContainerViewModel>()
        {
            new SourceContainerViewModel(connection, DbKeys.UserGeometries)
            {
                Header = DbKeys.UserGeometries.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source13" },
                    new SourceViewModel(source) { Name = "Source14" },
                    new SourceViewModel(source) { Name = "Source15" },
                },
            },
        };
        }

        connection.IsActive = true;

        return connection;
    }

    public static SidePanelViewModel SidePanel => CreateSidePanel();

    public static ScaleMapBar ScaleMapBar => CreateScaleMapBar();

    public static MainViewModel MainViewModel => CreateMainViewModel();

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

    private static SidePanelViewModel CreateSidePanel()
    {
        var sidePanel = new SidePanelViewModel();

        var tabs = new SidePanelTabViewModel[]
        {
            new FootprintPreviewTabViewModel(),
            new SatelliteTabViewModel(),
            new GroundStationTabViewModel(),
            new GroundTargetTabViewModel(),
            new FootprintTabViewModel(),
            new UserGeometryTabViewModel(),
        };

        sidePanel.Tabs.AddRange(tabs);

        return sidePanel;
    }

    private static MainViewModel CreateMainViewModel()
    {
        var mainViewModel = new MainViewModel();

        var tabs = new SidePanelTabViewModel[]
        {
            new FootprintPreviewTabViewModel(),
            new SatelliteTabViewModel(),
            new GroundStationTabViewModel(),
            new GroundTargetTabViewModel(),
            new FootprintTabViewModel(),
            new UserGeometryTabViewModel(),
        };

        mainViewModel.SidePanel.Tabs.AddRange(tabs);

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
    }

    public override string Title { get => "Select planned schedule"; protected set { } }

    public List<PlannedScheduleItemViewModel> Items { get; set; }

    public PlannedScheduleItemViewModel? SelectedItem { get; set; }
}
