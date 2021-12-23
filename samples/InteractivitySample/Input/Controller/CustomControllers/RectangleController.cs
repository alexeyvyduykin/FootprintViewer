namespace InteractivitySample.Input.Controller
{
    public class RectangleController : ControllerBase, IMapController
    {
        public RectangleController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingRectangle);
            this.BindMouseEnter(MapCommands.HoverDrawingRectangle);
        }
    }
}
