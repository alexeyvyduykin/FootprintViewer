using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Models;
using FootprintViewer.ViewModels;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        public static GroundTargetViewer? GroundTargetViewer { get; private set; }

        public static SatelliteViewer? SatelliteViewer { get; private set; }

        public static SceneSearch? SceneSearch { get; private set; }

        public static SceneSearchFilter? SceneSearchFilter { get; private set; }

        public static SidePanel? SidePanel { get; private set; }

        public static ToolBar? ToolBar { get; private set; }

        public static WorldMapSelector? WorldMapSelector { get; private set; }

        public static void InitializeContext()
        {
            // Map

            var map = new Mapsui.Map();

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

            FootprintObserver = new FootprintObserver(map)
            {
                Type = FootprintViewerContentType.Show,
                FootprintInfos = new ObservableCollection<FootprintInfo>()
                {
                    new FootprintInfo()
                    {
                        Name = "Footrpint0001",
                        IsShowInfo = false
                    },
                    new FootprintInfo()
                    {
                        Name = "Footrpint0002",
                        SatelliteName = "Satellite1",
                        IsShowInfo = true,
                        Center = new Coordinate(54.434545, -12.435454),
                        Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                        Duration = 35,
                        Node = 11,
                        Direction = SatelliteStripDirection.Left,
                    },
                    new FootprintInfo()
                    {
                        Name = "Footrpint0003",
                        IsShowInfo = false
                    },
                },
            };

            FootprintObserverList = new FootprintObserverList(null);

            FootprintObserverFilter = new FootprintObserverFilter()
            {
                FromNode = 1,
                ToNode = 15,
                Satellites = new ObservableCollection<SatelliteItem>(
                    new[]
                    {
                        new SatelliteItem() { Name = "Satellite1" },
                        new SatelliteItem() { Name = "Satellite2" },
                        new SatelliteItem() { Name = "Satellite3" },
                        new SatelliteItem() { Name = "Satellite4" },
                        new SatelliteItem() { Name = "Satellite5" }
                    })
            };

            var coll = new ObservableCollection<GroundTargetInfo>(new[]
                {
                    new GroundTargetInfo(){ Name = "g1" },
                    new GroundTargetInfo(){ Name = "g1" },
                    new GroundTargetInfo(){ Name = "g1" },
                });

            GroundTargetViewer = new GroundTargetViewer()
            {
                Type = TargetViewerContentType.Show,
                GroundTargetInfos = coll,
                SelectedGroundTargetInfo = coll.FirstOrDefault(),
            };
                             
            var dt = new DateTime(2000, 6, 1, 12, 0, 0);
            var sat1 = new Satellite()
            {
                Semiaxis = 6945.03,
                Eccentricity = 0.0,
                InclinationDeg = 97.65,
                ArgumentOfPerigeeDeg = 0.0,
                LongitudeAscendingNodeDeg = 0.0,
                RightAscensionAscendingNodeDeg = 0.0,
                Period = 5760.0,
                Epoch = dt,
                InnerHalfAngleDeg = 32,
                OuterHalfAngleDeg = 48
            };

            SatelliteViewer = new SatelliteViewer(new[]            
            {           
                new SatelliteInfo() { Name = "Satellite1", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },           
                new SatelliteInfo() { Name = "Satellite2", Satellite = sat1, IsShow = true, IsShowInfo = false, MaxNode = 15 },           
                new SatelliteInfo() { Name = "Satellite3", Satellite = sat1, IsShow = false, IsShowInfo = true, MaxNode = 15 },           
                new SatelliteInfo() { Name = "Satellite4", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },           
                new SatelliteInfo() { Name = "Satellite5", Satellite = sat1, IsShow = false, IsShowInfo = false, MaxNode = 15 },
            });

            SceneSearch = CreateSceneSearch();

            SceneSearchFilter = new SceneSearchFilter()
            {
                FromDate = DateTime.Today.AddDays(-1),
                ToDate = DateTime.Today.AddDays(1),
                Sensors = new ObservableCollection<Sensor>(new[]
                {
                    new Sensor() { Name = "Satellite1 SNS-1" },
                    new Sensor() { Name = "Satellite1 SNS-2" },
                    new Sensor() { Name = "Satellite2 SNS-1" },
                    new Sensor() { Name = "Satellite3 SNS-1" },
                    new Sensor() { Name = "Satellite3 SNS-2" },
                }),
            };

            // SidePanel

            var tabs = new SidePanelTab[]
            {
               
                new GroundTargetViewer()                   
                {                       
                    Name = "Test1",                        
                    Title = "Default test title1"                  
                },
                new GroundTargetViewer()                   
                {                       
                    Name = "Test2",                       
                    Title = "Default test title2"                   
                }
            };

            SidePanel = new SidePanel()
            {
                Tabs = new ObservableCollection<SidePanelTab>(tabs),
                SelectedTab = tabs.FirstOrDefault(),
            };

            // ToolManager

            var toolRectangle = new Tool()
            {
                Title = "AddRectangle",
            };

            var toolPolygon = new Tool()
            {
                Title = "AddPolygon",
            };

            var toolCircle = new Tool()
            {
                Title = "AddCircle",
            };

            ToolBar = new ToolBar()
            {
                AOICollection = new ToolCollection(new[] { toolRectangle, toolPolygon, toolCircle }) { Visible = true },
                RouteDistance = new Tool()
                {
                    Title = "Route",
                },
                Edit = new Tool()
                {
                    Title = "Edit",
                },
                WorldMaps = new Tool()
                {
                    Title = "WorldMaps",
                },
            };

            // WorldMapSelector

            WorldMapSelector = new WorldMapSelector(new[]
            {
                new LayerSource() { Name = "WorldMapDefault" },
                new LayerSource() { Name = "OAM-World-1-8-min-J70" },
                new LayerSource() { Name = "OAM-World-1-10-J70" }
            });
        }

        public static SceneSearch CreateSceneSearch()
        {
            var sceneSearch = new SceneSearch()
            {
                Name = "Scene",
                Title = "Поиск сцены",
            };

            var list = new List<FootprintImage>();

            Random random = new Random();

            var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
            var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

            foreach (var item in names)
            {
                var name = item.Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
                var date = DateTime.UtcNow;

                list.Add(new FootprintImage()
                {
                    Date = date.Date.ToShortDateString(),
                    SatelliteName = satellites[random.Next(0, satellites.Length - 1)],
                    SunElevation = random.Next(0, 90),
                    CloudCoverFull = random.Next(0, 100),
                    TileNumber = name.ToUpper(),
                });
            }

            sceneSearch.Footprints.Clear();
            sceneSearch.Footprints.AddRange(list);

            var sortNames = sceneSearch.Footprints.Select(s => s.SatelliteName).Distinct().ToList();
            sortNames.Sort();

            sceneSearch.Filter.AddSensors(sortNames);

            return sceneSearch;
        }
    }

    public class ObservableGroundTargetCollection : ObservableCollection<GroundTargetInfo> { }
}
