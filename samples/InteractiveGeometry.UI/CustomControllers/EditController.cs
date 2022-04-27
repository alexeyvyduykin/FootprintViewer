using InteractiveGeometry.UI.Input;

namespace InteractiveGeometry.UI
{
    public class EditController : ControllerBase, IMapController
    {
        public EditController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Editing);
            this.BindMouseEnter(MapCommands.HoverEditing);
        }
    }
}
