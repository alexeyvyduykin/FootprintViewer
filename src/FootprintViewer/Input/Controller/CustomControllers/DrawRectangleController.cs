namespace FootprintViewer
{
    public class DrawRectangleController : ControllerBase, IMapController
    {
        public DrawRectangleController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.DrawingRectangle);
            this.BindMouseEnter(MapCommands.HoverDrawingRectangle);
        }
    }
}
