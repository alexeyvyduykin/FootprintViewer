﻿using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FootprintViewer.Designer
{
    public class DesignerContext
    {
        public static RouteInfoPanel? RouteInfoPanel { get; private set; }

        public static AOIInfoPanel? AoiInfoPanel { get; private set; }

        public static InfoPanel? InfoPanel { get; private set; }

        public static PreviewMainContent? PreviewMainContent { get; private set; }

        public static FootprintObserver? FootprintObserver { get; private set; }

        public static FootprintObserverList? FootprintObserverList { get; private set; }

        public static FootprintObserverFilter? FootprintObserverFilter { get; private set; }

        public static SatelliteViewer? SatelliteViewer { get; private set; }

        public static SidePanel? SidePanel { get; private set; }

        public static ToolBar? ToolBar { get; private set; }

        public static WorldMapSelector? WorldMapSelector { get; private set; }

        public static MainViewModel? MainViewModel { get; private set; }

        public static GroundTargetInfo? GroundTargetInfo { get; private set; }

        public static SatelliteInfo? SatelliteInfo { get; private set; }

        public static void InitializeContext(IReadonlyDependencyResolver dependencyResolver)
        {
            // Map

            var map = dependencyResolver.GetService<Mapsui.Map>();

            // PreviewMainContent

            PreviewMainContent = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            // CustomInfoPanels

            RouteInfoPanel = new RouteInfoPanel()
            {
                Text = "Description",
            };

            AoiInfoPanel = new AOIInfoPanel()
            {
                Text = "Description",
            };

            // InfoPanel

            InfoPanel = new InfoPanel();

            InfoPanel.Show(RouteInfoPanel);
            InfoPanel.Show(AoiInfoPanel);

            // Tabs
            // Tabs: FootprintObserver

            FootprintObserver = dependencyResolver.GetExistingService<FootprintObserver>();

            FootprintObserver.IsActive = true;

            // Tabs: FootprintObserverList

            FootprintObserverList = new FootprintObserverList(dependencyResolver);

            FootprintObserverList.Update.Execute(null).Subscribe();

            // Tabs: FootprintObserverFilter

            FootprintObserverFilter = new FootprintObserverFilter(dependencyResolver);

            // Tabs: SatelliteViewer

            SatelliteViewer = dependencyResolver.GetService<SatelliteViewer>();

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

            // WorldMapSelector

            WorldMapSelector = new WorldMapSelector(dependencyResolver);

            // MainViewModel

            MainViewModel = dependencyResolver.GetExistingService<MainViewModel>();

            MainViewModel.SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));

            // GroundTargetInfo

            GroundTargetInfo = new GroundTargetInfo(new GroundTarget()
            {
                Name = "GroundTarget001",
                Type = GroundTargetType.Route,
            });

            // SatelliteInfo

            SatelliteInfo = new SatelliteInfo(new Satellite()
            {
                Name = "Satellite1",
                Semiaxis = 6945.03,
                Eccentricity = 0.0,
                InclinationDeg = 97.65,
                ArgumentOfPerigeeDeg = 0.0,
                LongitudeAscendingNodeDeg = 0.0,
                RightAscensionAscendingNodeDeg = 0.0,
                Period = 5760.0,
                Epoch = new DateTime(2000, 6, 1, 12, 0, 0),
                InnerHalfAngleDeg = 32,
                OuterHalfAngleDeg = 48
            })
            {
                IsShow = true,
                IsShowInfo = true,
            };
        }
    }

    public class ObservableGroundTargetCollection : ObservableCollection<GroundTargetInfo> { }
}
