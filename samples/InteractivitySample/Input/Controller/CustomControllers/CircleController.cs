namespace InteractivitySample.Input.Controller
{
    public class CircleController : ControllerBase, IMapController
    {
        public CircleController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingCircle);
            this.BindMouseEnter(MapCommands.HoverDrawingCircle);
        }
    }
}
