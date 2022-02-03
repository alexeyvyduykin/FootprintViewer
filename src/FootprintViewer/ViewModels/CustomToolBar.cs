using FootprintViewer.Data;
using FootprintViewer.Models;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using Splat;
using System;
using FootprintViewer.ViewModels;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class CustomToolBar : ToolBar
    {
        private readonly WorldMapSelector _worldMapSelector;

        private readonly ToolCheck _addRectangle;
        private readonly ToolCheck _addPolygon;
        private readonly ToolCheck _addCircle;

        private readonly ToolCheck _rectangle;
        private readonly ToolCheck _circle;
        private readonly ToolCheck _polygon;

        public CustomToolBar(IReadonlyDependencyResolver dependencyResolver) : base()
        {
            _worldMapSelector = new WorldMapSelector(dependencyResolver);        
            _worldMapSelector.WorldMapChanged.Subscribe(_ => IsWorldMapSelectorOpen = false);
        
            ZoomIn = new ToolClick()
            {
                Title = "ZoomIn",
                Tooltip = "Приблизить",
            };

            ZoomOut = new ToolClick()
            {
                Title = "ZoomOut",
                Tooltip = "Отдалить",
            };

            _addRectangle = new ToolCheck()
            {
                Title = "AddRectangle",
                Tooltip = "Нарисуйте прямоугольную AOI",
                Group = "Group1",
            };

            _addPolygon = new ToolCheck()
            {
                Title = "AddPolygon",
                Tooltip = "Нарисуйте полигональную AOI",
                Group = "Group1",
            };

            _addCircle = new ToolCheck()
            {
                Title = "AddCircle",
                Tooltip = "Нарисуйте круговую AOI",
                Group = "Group1",
            };

            AOICollection = new ToolCollection();
            AOICollection.AddItem(_addRectangle);
            AOICollection.AddItem(_addPolygon);
            AOICollection.AddItem(_addCircle);

            RouteDistance = new ToolCheck()
            {
                Title = "Route",
                Tooltip = "Измерить расстояние",
                Group = "Group1",
            };

            WorldMaps = new ToolClick()
            {
                Title = "WorldMaps",
                Tooltip = "Список слоев",          
            };

            WorldMaps.Click.Subscribe(_ => { IsWorldMapSelectorOpen = !IsWorldMapSelectorOpen; });

            SelectGeometry = new ToolCheck()
            {
                Title = "Select",
                Tooltip = "SelectGeometry",
                Group = "Group2",
            };

            _rectangle = new ToolCheck()
            {
                Title = "Rectangle",
                Tooltip = "RectangleGeometry",
                Group = "Group2",
            };

            _circle = new ToolCheck()
            {
                Title = "Circle",
                Tooltip = "CircleGeometry",
                Group = "Group2",
            };

            _polygon = new ToolCheck()
            {
                Title = "Polygon",
                Tooltip = "PolygonGeometry",
                Group = "Group2",
            };

            GeometryCollection = new ToolCollection();
            GeometryCollection.AddItem(_rectangle);
            GeometryCollection.AddItem(_circle);
            GeometryCollection.AddItem(_polygon);

            TranslateGeometry = new ToolCheck()
            {
                Title = "Translate",
                Tooltip = "TranslateGeometry",
                Group = "Group2",
            };

            RotateGeometry = new ToolCheck()
            {
                Title = "Rotate",
                Tooltip = "RotateGeometry",
                Group = "Group2",
            };

            ScaleGeometry = new ToolCheck()
            {
                Title = "Scale",
                Tooltip = "ScaleGeometry",
                Group = "Group2",
            };

            EditGeometry = new ToolCheck()
            {
                Title = "Edit",
                Tooltip = "EditGeometry",
                Group = "Group2",
            };

            AddTool(ZoomIn);
            AddTool(ZoomOut);
            AddTool(AOICollection);
            AddTool(RouteDistance);
            AddTool(SelectGeometry);
            AddTool(GeometryCollection);
            AddTool(TranslateGeometry);
            AddTool(RotateGeometry);
            AddTool(ScaleGeometry);
            AddTool(EditGeometry);

            ZoomInClick = ZoomIn.Click;

            ZoomOutClick = ZoomOut.Click;

            AddRectangleCheck = _addRectangle.Check.Where(s => s.IsCheck == true);

            AddPolygonCheck = _addPolygon.Check.Where(s => s.IsCheck == true);

            AddCircleCheck = _addCircle.Check.Where(s => s.IsCheck == true);

            RouteDistanceCheck = RouteDistance.Check.Where(s => s.IsCheck == true);

            SelectGeometryCheck = SelectGeometry.Check;

            RectangleGeometryCheck = _rectangle.Check;

            CircleGeometryCheck = _circle.Check;

            PolygonGeometryCheck = _polygon.Check;

            TranslateGeometryCheck = TranslateGeometry.Check;

            RotateGeometryCheck = RotateGeometry.Check;

            ScaleGeometryCheck = ScaleGeometry.Check;

            EditGeometryCheck = EditGeometry.Check;
        }

        public void Uncheck()
        {
            foreach (var item in Tools)
            {
                if (item is IToolCheck check)
                {
                    if (check.IsCheck == true)
                    {
                        check.Check.Execute(false).Subscribe();
                    }
                }
                else if (item is IToolCollection collection)
                {
                    foreach (var itemCheck in collection.Items)
                    {
                        if (itemCheck.IsCheck == true)
                        {
                            itemCheck.Check.Execute(false).Subscribe();
                        }
                    }
                }
            }
        }

        public IObservable<IToolClick> ZoomInClick { get; }

        public IObservable<IToolClick> ZoomOutClick { get; }

        public IObservable<IToolCheck> AddRectangleCheck { get; }

        public IObservable<IToolCheck> AddPolygonCheck { get; }

        public IObservable<IToolCheck> AddCircleCheck { get; }

        public IObservable<IToolCheck> RouteDistanceCheck { get; }

        public IObservable<IToolCheck> SelectGeometryCheck { get; }

        public IObservable<IToolCheck> RectangleGeometryCheck { get; }

        public IObservable<IToolCheck> CircleGeometryCheck { get; }

        public IObservable<IToolCheck> PolygonGeometryCheck { get; }

        public IObservable<IToolCheck> TranslateGeometryCheck { get; }

        public IObservable<IToolCheck> RotateGeometryCheck { get; }

        public IObservable<IToolCheck> ScaleGeometryCheck { get; }

        public IObservable<IToolCheck> EditGeometryCheck { get; }

        public ToolClick ZoomIn { get; }

        public ToolClick ZoomOut { get; }

        public IToolCollection AOICollection { get; }

        public ToolCheck RouteDistance { get; }
        
        public ToolClick WorldMaps { get; }

        public ToolCheck SelectGeometry { get; }

        public IToolCollection GeometryCollection { get; }

        public ToolCheck TranslateGeometry { get; }

        public ToolCheck RotateGeometry { get; }

        public ToolCheck ScaleGeometry { get; }

        public ToolCheck EditGeometry { get; }

        public ReactiveCommand<MapResource, MapResource> LayerChanged => _worldMapSelector.WorldMapChanged;

        public WorldMapSelector WorldMapSelector => _worldMapSelector;

        [Reactive]
        public bool IsWorldMapSelectorOpen { get; set; }
    }
}
