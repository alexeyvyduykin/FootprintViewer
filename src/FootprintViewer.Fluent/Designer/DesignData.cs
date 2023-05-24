﻿using FootprintViewer.Data.Builders;
using FootprintViewer.Fluent.ViewModels;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;
using FootprintViewer.Fluent.ViewModels.InfoPanel;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Fluent.ViewModels.Settings;
using FootprintViewer.Fluent.ViewModels.SidePanel;
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

    public static ToolBarViewModel ToolBar => new(_resolver);

    public static LayerContainerViewModel LayerContainer => new(_resolver);

    public static CustomTipViewModel CustomTip => CustomTipViewModel.HoverCreating(TipTarget.Rectangle, 34545.432);

    public static TimelinesViewModel Timelines => new();

    public static TimelinesOldViewModel TimelinesOld => new();

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

        panel.Show(InfoPanelItemViewModel.Create("Route", "Route description"));
        panel.Show(InfoPanelItemViewModel.Create("AOI", "AOI description"));
        panel.Show(FootprintInfoPanelItemViewModel.Create(new(FootprintBuilder.CreateRandom())));
        panel.Show(GroundTargetInfoPanelItemViewModel.Create(new(GroundTargetBuilder.CreateRandom())));
        panel.Show(UserGeometryInfoPanelItemViewModel.Create(new(UserGeometryBuilder.CreateRandom())));

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