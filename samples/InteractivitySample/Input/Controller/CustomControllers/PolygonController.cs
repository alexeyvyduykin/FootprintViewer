namespace InteractivitySample.Input.Controller
{
    public class PolygonController : ControllerBase, IMapController
    {
        public PolygonController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingPolygon);
            this.BindMouseEnter(MapCommands.HoverDrawingPolygon);
        }
    }
}
