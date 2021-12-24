namespace InteractivitySample.Input.Controller
{
    public class DrawingController : ControllerBase, IMapController
    {
        public DrawingController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Drawing);
            this.BindMouseEnter(MapCommands.HoverDrawing);
        }
    }
}
