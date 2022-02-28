namespace FootprintViewer.Input
{
    public class DefaultController : ControllerBase, IMapController
    {
        public DefaultController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Default);
        }
    }
}
