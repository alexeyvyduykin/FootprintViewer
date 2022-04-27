using InteractiveGeometry.UI.Input;

namespace InteractiveGeometry.UI
{
    public class DefaultController : ControllerBase, IMapController
    {
        public DefaultController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Default);
            this.BindMouseEnter(MapCommands.HoverDefault);
        }
    }
}
