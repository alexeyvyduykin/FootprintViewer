namespace FootprintViewer
{
    public class DrawCircleController : ControllerBase, IMapController
    {
        public DrawCircleController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingCircle);
            this.BindMouseEnter(MapCommands.HoverDrawingCircle);
        }
    }
}
