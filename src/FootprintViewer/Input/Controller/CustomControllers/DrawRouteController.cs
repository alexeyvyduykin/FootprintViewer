namespace FootprintViewer
{
    public class DrawRouteController : ControllerBase, IMapController
    {
        public DrawRouteController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingLine);
            this.BindMouseEnter(MapCommands.HoverDrawingLine);
        }
    }
}
