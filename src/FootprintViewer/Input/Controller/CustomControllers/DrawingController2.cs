namespace FootprintViewer
{
    public class DrawingController2 : ControllerBase, IMapController
    {
        public DrawingController2()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Drawing2);
            this.BindMouseEnter(MapCommands.HoverDrawing2);
        }
    }
}
