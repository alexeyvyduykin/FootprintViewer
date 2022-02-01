using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public class ToolBar : ReactiveObject
    {
        private readonly WorldMapSelector _worldMapSelector;

        public ToolBar(IReadonlyDependencyResolver dependencyResolver)
        {
            _worldMapSelector = new WorldMapSelector(dependencyResolver);

            ZoomInClick = ReactiveCommand.Create(() => { });
            ZoomOutClick = ReactiveCommand.Create(() => { });
            RectangleClick = ReactiveCommand.Create(() => { });
            PolygonClick = ReactiveCommand.Create(() => { });
            CircleClick = ReactiveCommand.Create(() => { });
            RouteDistanceClick = ReactiveCommand.Create(() => { });
            EditClick = ReactiveCommand.Create(() => { });
            var worldMapsClick = ReactiveCommand.Create(() => { });

            SelectGeometryClick = ReactiveCommand.Create(() => { });
            RectangleGeometryClick = ReactiveCommand.Create(() => { });       
            CircleGeometryClick = ReactiveCommand.Create(() => { });
            PolygonGeometryClick = ReactiveCommand.Create(() => { });
            TranslateGeometryClick = ReactiveCommand.Create(() => { });
            RotateGeometryClick = ReactiveCommand.Create(() => { });
            ScaleGeometryClick = ReactiveCommand.Create(() => { });
            EditGeometryClick = ReactiveCommand.Create(() => { });

            ZoomIn = new Tool()
            {
                Title = "+",
                Tooltip = "Приблизить",
                Command = ZoomInClick,
            };

            ZoomOut = new Tool()
            {
                Title = "-",
                Tooltip = "Отдалить",
                Command = ZoomOutClick,
            };

            AOICollection = new ToolCollection(new[]
            {
                new Tool()
                {
                    Title = "AddRectangle",
                    Tooltip = "Нарисуйте прямоугольную AOI",
                    Command = RectangleClick,
                },
                new Tool()
                {
                    Title = "AddPolygon",
                    Tooltip = "Нарисуйте полигональную AOI",
                    Command = PolygonClick,
                },
                new Tool()
                {
                    Title = "AddCircle",
                    Tooltip = "Нарисуйте круговую AOI",
                    Command = CircleClick,
                }
            });

            RouteDistance = new Tool()
            {
                Title = "Route",
                Tooltip = "Измерить расстояние",
                Command = RouteDistanceClick,
            };

            Edit = new Tool()
            {
                Title = "Edit",
                Command = EditClick,
            };

            WorldMaps = new Tool()
            {
                Title = "WorldMaps",
                Tooltip = "Список слоев",
                Command = worldMapsClick,
            };

            _worldMapSelector.WorldMapChanged.Subscribe(_ => IsWorldMapSelectorOpen = false);

            SelectGeometry = new Tool()
            {
                Title = "Select",
                Tooltip = "SelectGeometry",
                Command = SelectGeometryClick,
            };

            GeometryCollection = new ToolCollection(new[]
            {
                new Tool()
                {
                    Title = "Rectangle",
                    Tooltip = "RectangleGeometry",
                    Command = RectangleGeometryClick,
                },                                
                new Tool()
                {
                    Title = "Circle",
                    Tooltip = "CircleGeometry",
                    Command = CircleGeometryClick,
                },               
                new Tool()
                {
                    Title = "Polygon",
                    Tooltip = "PolygonGeometry",
                    Command = PolygonGeometryClick,
                },
            });

            TranslateGeometry = new Tool()
            {
                Title = "Translate",
                Tooltip = "TranslateGeometry",
                Command = TranslateGeometryClick,
            };

            RotateGeometry = new Tool() 
            {
                Title = "Rotate",
                Tooltip = "RotateGeometry",
                Command = RotateGeometryClick,
            };

            ScaleGeometry = new Tool() 
            {
                Title = "Scale",
                Tooltip = "ScaleGeometry",
                Command = ScaleGeometryClick,
            };

            EditGeometry = new Tool()
            {
                Title = "Edit",
                Tooltip = "EditGeometry",
                Command = EditGeometryClick,
            };

            worldMapsClick.Subscribe(_ => { IsWorldMapSelectorOpen = !IsWorldMapSelectorOpen; });

            this.WhenAnyValue(x => x.RouteDistance.IsActive).Subscribe(isActive =>
            {
                if (isActive == true)
                {
                    if (AOICollection != null)
                    {
                        AOICollection.IsActive = false;
                    }
                }
                else
                {
                    if (Edit != null)
                    {
                        Edit.Command?.Execute().Subscribe();
                    }
                }
            });

            this.WhenAnyValue(x => x.AOICollection.IsActive).Subscribe(isActive =>
            {
                if (isActive == true)
                {
                    if (RouteDistance != null)
                    {
                        RouteDistance.IsActive = false;
                    }
                }
                else
                {
                    if (Edit != null)
                    {
                        Edit.Command?.Execute().Subscribe();
                    }
                }
            });
        }

        public void ResetAllTools()
        {
            if (AOICollection.IsActive == true)
            {
                AOICollection.IsActive = false;
            }

            if (RouteDistance.IsActive == true)
            {
                RouteDistance.IsActive = false;
            }
        }

        public ReactiveCommand<Unit, Unit> ZoomInClick { get; }

        public ReactiveCommand<Unit, Unit> ZoomOutClick { get; }

        public ReactiveCommand<Unit, Unit> RectangleClick { get; }

        public ReactiveCommand<Unit, Unit> PolygonClick { get; }

        public ReactiveCommand<Unit, Unit> CircleClick { get; }

        public ReactiveCommand<Unit, Unit> RouteDistanceClick { get; }

        public ReactiveCommand<Unit, Unit> EditClick { get; }

        public ReactiveCommand<Unit, Unit> SelectGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> RectangleGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> CircleGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> PolygonGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> TranslateGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> RotateGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> ScaleGeometryClick { get; }

        public ReactiveCommand<Unit, Unit> EditGeometryClick { get; }

        public ReactiveCommand<MapResource, MapResource> LayerChanged => _worldMapSelector.WorldMapChanged;

        public Tool ZoomIn { get; }

        public Tool ZoomOut { get; }

        public ToolCollection AOICollection { get; }

        public Tool RouteDistance { get; }

        public Tool Edit { get; }

        public Tool WorldMaps { get; }

        public WorldMapSelector WorldMapSelector => _worldMapSelector;

        public Tool SelectGeometry { get; }

        public ToolCollection GeometryCollection { get; }

        public Tool TranslateGeometry { get; }

        public Tool RotateGeometry { get; }

        public Tool ScaleGeometry { get; }

        public Tool EditGeometry { get; }

        [Reactive]
        public bool IsWorldMapSelectorOpen { get; set; }
    }
}
