﻿using FootprintViewer.Data;
using FootprintViewer.Data.Builders;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.ViewModels;
using FootprintViewer.ViewModels.Settings;
using FootprintViewer.ViewModels.Settings.SourceBuilders;
using FootprintViewer.ViewModels.SidePanel;
using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Items;
using FootprintViewer.ViewModels.SidePanel.Tabs;
using FootprintViewer.ViewModels.Timelines;
using FootprintViewer.ViewModels.Tips;
using FootprintViewer.ViewModels.ToolBar;
using Mapsui;
using Splat;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FootprintViewer.Designer;

public static class DesignData
{
    private static readonly IReadonlyDependencyResolver _resolver = new DesignDataDependencyResolver();

    public static FootprintPreviewTabFilterViewModel FootprintPreviewFilter => new(_resolver);

    public static FootprintTabFilterViewModel FootprintFilter => new(_resolver);

    public static GroundTargetTabFilterViewModel GroundTargetFilter => new(_resolver);

    public static BottomPanel BottomPanel => new(_resolver);

    public static SnapshotMaker SnapshotMaker => new(_resolver);

    public static ToolBarViewModel ToolBar => new(_resolver);

    public static LayerContainerViewModel LayerContainer => new(_resolver);

    public static CustomTipViewModel CustomTip => CustomTipViewModel.BeginCreating(TipTarget.Rectangle, 34545.432);

    public static FootprintPreviewTabViewModel FootprintPreviewTab => new(_resolver) { IsActive = true };

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

    public static TimelinesViewModel Timelines => new(_resolver);

    public static TimelinesOldViewModel TimelinesOld => new(_resolver);

    public static FootprintClickInfoPanel FootprintClickInfoPanel => new(new FootprintViewModel(FootprintBuilder.CreateRandom()));

    public static GroundTargetClickInfoPanel GroundTargetClickInfoPanel => new(new GroundTargetViewModel(GroundTargetBuilder.CreateRandom()));

    public static RouteInfoPanel RouteInfoPanel => new() { Text = "Description" };

    public static AOIInfoPanel AOIInfoPanel => new() { Text = "Description" };

    public static UserGeometryClickInfoPanel UserGeometryClickInfoPanel => new(new UserGeometryViewModel(UserGeometryBuilder.CreateRandom()));

    public static InfoPanel InfoPanel => CreateInfoPanel();

    private static InfoPanel CreateInfoPanel()
    {
        var panel = new InfoPanel();

        panel.Show(RouteInfoPanel);
        panel.Show(AOIInfoPanel);
        panel.Show(FootprintClickInfoPanel);
        panel.Show(GroundTargetClickInfoPanel);
        panel.Show(UserGeometryClickInfoPanel);

        return panel;
    }

    public static TableInfoViewModel TableInfo => TableInfoViewModel.Build(TableInfoType.Footprint);

    public static SettingsViewModel Settings => new(_resolver) { IsActive = true };

    public static DatabaseBuilderViewModel DatabaseBuilder => new(DbKeys.Footprints.ToString())
    {
        Host = "localhost",
        Port = 5432,
        Database = "DataSettingsSampleDatabase1",
        Username = "postgres",
        Password = "user",
        IsActive = true
    };

    public static JsonBuilderViewModel JsonBuilder => new(DbKeys.Footprints.ToString())
    {
        Directory = GetFullPathToAssets(),
        IsActive = true
    };

    public static ConnectionViewModel Connection => CreateConnection();

    public static SourceContainerViewModel SourceContainer => CreateSourceContainer();

    private static string GetFullPathToAssets()
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var path = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));
        return path;
    }

    private static ConnectionViewModel CreateConnection()
    {
        var connection = new ConnectionViewModel(_resolver);

        var source = _resolver.GetService<IDataManager>()?.GetSources(DbKeys.Footprints.ToString())[0];

        if (source is not null)
        {
            connection.SourceContainers = new List<SourceContainerViewModel>()
        {
            new SourceContainerViewModel(connection, DbKeys.Footprints.ToString(), _resolver)
            {
                Header = DbKeys.Footprints.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source1" },
                    new SourceViewModel(source) { Name = "Source2" },
                    new SourceViewModel(source) { Name = "Source3" },
                },
            },
            new SourceContainerViewModel(connection, DbKeys.GroundStations.ToString(), _resolver)
            {
                Header = DbKeys.GroundStations.ToString(),
                Sources = new List<ISourceViewModel>()
                {
                    new SourceViewModel(source) { Name = "Source10" },
                    new SourceViewModel(source) { Name = "Source11" },
                    new SourceViewModel(source) { Name = "Source12" },
                },
            },
            new SourceContainerViewModel(connection, DbKeys.UserGeometries.ToString(), _resolver)
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

    private static SourceContainerViewModel CreateSourceContainer()
    {
        var container = new SourceContainerViewModel(Connection, DbKeys.Footprints.ToString(), _resolver)
        {
            Header = DbKeys.Footprints.ToString()
        };

        var source = _resolver.GetService<IDataManager>()?.GetSources(DbKeys.Footprints.ToString())[0];

        if (source is not null)
        {
            container.Sources = new List<ISourceViewModel>()
            {
                new SourceViewModel(source) { Name = "Source1" },
                new SourceViewModel(source) { Name = "Source2" },
                new SourceViewModel(source) { Name = "Source3" },
            };
        }

        return container;
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
            new FootprintPreviewTabViewModel(_resolver),
            new SatelliteTabViewModel(_resolver),
            new GroundStationTabViewModel(_resolver),
            new GroundTargetTabViewModel(_resolver),
            new FootprintTabViewModel(_resolver),
            new UserGeometryTabViewModel(_resolver),
        };

        sidePanel.Tabs.AddRange(tabs);

        return sidePanel;
    }

    private static MainViewModel CreateMainViewModel()
    {
        var mainViewModel = new MainViewModel(_resolver);

        var tabs = new SidePanelTabViewModel[]
        {
            new FootprintPreviewTabViewModel(_resolver),
            new SatelliteTabViewModel(_resolver),
            new GroundStationTabViewModel(_resolver),
            new GroundTargetTabViewModel(_resolver),
            new FootprintTabViewModel(_resolver),
            new UserGeometryTabViewModel(_resolver),
        };

        mainViewModel.SidePanel.Tabs.AddRange(tabs);

        return mainViewModel;
    }
}
