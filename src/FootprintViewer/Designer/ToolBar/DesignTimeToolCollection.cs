using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeToolCollection : ToolCollection
    {
        public DesignTimeToolCollection() : base()
        {
            var rectangle = new ToolCheck()
            {
                Title = "Rectangle",
                Tooltip = "RectangleGeometry",
                Group = "Group1",
            };

            var circle = new ToolCheck()
            {
                Title = "Circle",
                Tooltip = "CircleGeometry",
                Group = "Group1",
            };

            var polygon = new ToolCheck()
            {
                Title = "Polygon",
                Tooltip = "PolygonGeometry",
                Group = "Group1",
            };

            AddItem(rectangle);
            AddItem(circle);
            AddItem(polygon);
        }
    }
}
