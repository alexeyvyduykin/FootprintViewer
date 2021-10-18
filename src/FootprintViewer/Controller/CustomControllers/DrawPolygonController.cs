namespace FootprintViewer
{
    public class DrawPolygonController : ControllerBase, IMapController
    {
        public DrawPolygonController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingPolygon);
            this.BindMouseEnter(MapCommands.HoverDrawingPolygon);
        }
    }
}
