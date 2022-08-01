using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeToolCollection : ToolCollection
    {
        public DesignTimeToolCollection() : base()
        {
            var rectangle = new ToolCheck()
            {
                Tag = "Rectangle",
                Group = "Group1",
            };

            var circle = new ToolCheck()
            {
                Tag = "Circle",
                Group = "Group1",
            };

            var polygon = new ToolCheck()
            {
                Tag = "Polygon",
                Group = "Group1",
            };

            AddItem(rectangle);
            AddItem(circle);
            AddItem(polygon);
        }
    }
}
