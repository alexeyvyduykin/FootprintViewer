using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Models;
using FootprintViewer.ViewModels;
using NetTopologySuite.Geometries;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        public static MainViewModel? MainViewModel { get; private set; }

        public static FootprintInfo? FootprintInfo { get; private set; }

        public static GroundTargetInfo? GroundTargetInfo { get; private set; }

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

            FootprintObserver = dependencyResolver.GetService<FootprintObserver>();

            // Tabs: FootprintObserverList

            FootprintObserverList = new FootprintObserverList(dependencyResolver);

            FootprintObserverList?.LoadFootprints.Execute().Subscribe();

            // Tabs: FootprintObserverFilter

            FootprintObserverFilter = new FootprintObserverFilter(dependencyResolver);

            // Tabs: GroundTargetViewer

            GroundTargetViewer = dependencyResolver.GetService<GroundTargetViewer>();

            GroundTargetViewer?.UpdateAll();

            // Tabs: SatelliteViewer

            SatelliteViewer = dependencyResolver.GetService<SatelliteViewer>();

            // Tabs: SceneSearch

            SceneSearch = CreateSceneSearch(dependencyResolver);

            // Tabs: SceneSearchFilter

            SceneSearchFilter = new SceneSearchFilter(dependencyResolver)
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

            WorldMapSelector = new WorldMapSelector(dependencyResolver);

            // MainViewModel

            MainViewModel = dependencyResolver.GetService<MainViewModel>();

            MainViewModel?.SidePanel.Tabs.AddRange(new List<SidePanelTab>(tabs));

            // FootprintInfo

            FootprintInfo = new FootprintInfo(new Footprint()
            {
                Name = "Footrpint001",
                SatelliteName = "Satellite1",
                Center = new Point(54.434545, -12.435454),
                Begin = new DateTime(2001, 6, 1, 12, 0, 0),
                Duration = 35,
                Node = 11,
                Direction = SatelliteStripDirection.Left,
            })
            {           
                IsShowInfo = true
            };

            // GroundTargetInfo

            GroundTargetInfo = new GroundTargetInfo(new GroundTarget() 
            {
                Name = "GroundTarget001",
                Type = GroundTargetType.Route,              
            });
        }

        public static SceneSearch CreateSceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            var sceneSearch = new SceneSearch(dependencyResolver)
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
