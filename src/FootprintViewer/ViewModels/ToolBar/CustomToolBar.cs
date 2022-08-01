using FootprintViewer.Models;
using Splat;

namespace FootprintViewer.ViewModels
{
    public class CustomToolBar : ToolBar
    {
        private readonly MapBackgroundList _mapBackgroundList;
        private readonly MapLayerList _mapLayerList;

        public CustomToolBar(IReadonlyDependencyResolver dependencyResolver) : base()
        {
            _mapBackgroundList = dependencyResolver.GetExistingService<MapBackgroundList>();

            _mapLayerList = dependencyResolver.GetExistingService<ProjectFactory>().CreateMapLayerList();

            ZoomIn = new ToolClick()
            {
                Tag = "ZoomIn",
            };

            ZoomOut = new ToolClick()
            {
                Tag = "ZoomOut",
            };

            AddRectangle = new ToolCheck()
            {
                Tag = "AddRectangle",
                Group = "Group1",
            };

            AddPolygon = new ToolCheck()
            {
                Tag = "AddPolygon",
                Group = "Group1",
            };

            AddCircle = new ToolCheck()
            {
                Tag = "AddCircle",
                Group = "Group1",
            };

            AOICollection = new ToolCollection();
            AOICollection.AddItem(AddRectangle);
            AOICollection.AddItem(AddPolygon);
            AOICollection.AddItem(AddCircle);

            RouteDistance = new ToolCheck()
            {
                Tag = "Route",
                Group = "Group1",
            };

            MapBackgrounds = new ToolCheck()
            {
                Tag = "MapBackgrounds",
            };

            MapLayers = new ToolCheck()
            {
                Tag = "MapLayers",
            };

            SelectGeometry = new ToolCheck()
            {
                Tag = "Select",
                Group = "Group1",
            };

            Point = new ToolCheck()
            {
                Tag = "Point",
                Group = "Group1",
            };

            Rectangle = new ToolCheck()
            {
                Tag = "Rectangle",
                Group = "Group1",
            };

            Circle = new ToolCheck()
            {
                Tag = "Circle",
                Group = "Group1",
            };

            Polygon = new ToolCheck()
            {
                Tag = "Polygon",
                Group = "Group1",
            };

            GeometryCollection = new ToolCollection();
            GeometryCollection.AddItem(Point);
            GeometryCollection.AddItem(Rectangle);
            GeometryCollection.AddItem(Circle);
            GeometryCollection.AddItem(Polygon);

            TranslateGeometry = new ToolCheck()
            {
                Tag = "Translate",
                Group = "Group1",
            };

            RotateGeometry = new ToolCheck()
            {
                Tag = "Rotate",
                Group = "Group1",
            };

            ScaleGeometry = new ToolCheck()
            {
                Tag = "Scale",
                Group = "Group1",
            };

            EditGeometry = new ToolCheck()
            {
                Tag = "Edit",
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

        public ToolCheck MapBackgrounds { get; }

        public ToolCheck MapLayers { get; }

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

        public MapBackgroundList MapBackgroundList => _mapBackgroundList;

        public MapLayerList MapLayerList => _mapLayerList;
    }
}
