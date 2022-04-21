namespace InteractiveSample.Input.Controller
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
