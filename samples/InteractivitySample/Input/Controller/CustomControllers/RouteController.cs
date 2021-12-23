namespace InteractivitySample.Input.Controller
{
    public class RouteController : ControllerBase, IMapController
    {
        public RouteController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingRoute);
            this.BindMouseEnter(MapCommands.HoverDrawingRoute);
        }
    }
}
