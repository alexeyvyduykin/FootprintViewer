using FootprintViewer.Models;
using Splat;

namespace FootprintViewer.ViewModels
{
    public class CustomToolBar : ToolBar
    {
        private readonly WorldMapSelector _worldMapSelector;

        public CustomToolBar(IReadonlyDependencyResolver dependencyResolver) : base()
        {
            _worldMapSelector = dependencyResolver.GetExistingService<WorldMapSelector>();

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

            AddRectangle = new ToolCheck()
            {
                Title = "AddRectangle",
                Tooltip = "Нарисуйте прямоугольную AOI",
                Group = "Group1",
            };

            AddPolygon = new ToolCheck()
            {
                Title = "AddPolygon",
                Tooltip = "Нарисуйте полигональную AOI",
                Group = "Group1",
            };

            AddCircle = new ToolCheck()
            {
                Title = "AddCircle",
                Tooltip = "Нарисуйте круговую AOI",
                Group = "Group1",
            };

            AOICollection = new ToolCollection();
            AOICollection.AddItem(AddRectangle);
            AOICollection.AddItem(AddPolygon);
            AOICollection.AddItem(AddCircle);

            RouteDistance = new ToolCheck()
            {
                Title = "Route",
                Tooltip = "Измерить расстояние",
                Group = "Group1",
            };

            WorldMaps = new ToolCheck()
            {
                Title = "WorldMaps",
                Tooltip = "Список слоев",
            };

            SelectGeometry = new ToolCheck()
            {
                Title = "Select",
                Tooltip = "SelectGeometry",
                Group = "Group1",
            };

            Point = new ToolCheck()
            {
                Title = "Point",
                Tooltip = "PointGeometry",
                Group = "Group1",
            };

            Rectangle = new ToolCheck()
            {
                Title = "Rectangle",
                Tooltip = "RectangleGeometry",
                Group = "Group1",
            };

            Circle = new ToolCheck()
            {
                Title = "Circle",
                Tooltip = "CircleGeometry",
                Group = "Group1",
            };

            Polygon = new ToolCheck()
            {
                Title = "Polygon",
                Tooltip = "PolygonGeometry",
                Group = "Group1",
            };

            GeometryCollection = new ToolCollection();
            GeometryCollection.AddItem(Point);
            GeometryCollection.AddItem(Rectangle);
            GeometryCollection.AddItem(Circle);
            GeometryCollection.AddItem(Polygon);

            TranslateGeometry = new ToolCheck()
            {
                Title = "Translate",
                Tooltip = "TranslateGeometry",
                Group = "Group1",
            };

            RotateGeometry = new ToolCheck()
            {
                Title = "Rotate",
                Tooltip = "RotateGeometry",
                Group = "Group1",
            };

            ScaleGeometry = new ToolCheck()
            {
                Title = "Scale",
                Tooltip = "ScaleGeometry",
                Group = "Group1",
            };

            EditGeometry = new ToolCheck()
            {
                Title = "Edit",
                Tooltip = "EditGeometry",
                Group = "Group1",
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
        }

        public void Uncheck()
        {
            foreach (var item in Tools)
            {
                if (item is IToolCheck check)
                {
                    check.IsCheck = false;
                }
                else if (item is IToolCollection collection)
                {
                    foreach (var itemCheck in collection.Items)
                    {
                        if (itemCheck is IToolCheck toolCheck)
                        {
                            toolCheck.IsCheck = false;
                        }
                    }
                }
            }
        }

        public ToolClick ZoomIn { get; }

        public ToolClick ZoomOut { get; }

        public IToolCollection AOICollection { get; }

        public ToolCheck RouteDistance { get; }

        public ToolCheck WorldMaps { get; }

        public ToolCheck SelectGeometry { get; }

        public IToolCollection GeometryCollection { get; }

        public ToolCheck TranslateGeometry { get; }

        public ToolCheck RotateGeometry { get; }

        public ToolCheck ScaleGeometry { get; }

        public ToolCheck EditGeometry { get; }

        public ToolCheck AddRectangle { get; }

        public ToolCheck AddPolygon { get; }

        public ToolCheck AddCircle { get; }

        public ToolCheck Point { get; }

        public ToolCheck Rectangle { get; }

        public ToolCheck Circle { get; }

        public ToolCheck Polygon { get; }

        public WorldMapSelector WorldMapSelector => _worldMapSelector;
    }
}
