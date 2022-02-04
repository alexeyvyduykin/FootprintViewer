using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public class EditController2 : ControllerBase, IMapController
    {
        public EditController2()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Editing2);
            this.BindMouseEnter(MapCommands.HoverEditing2);
        }
    }
}
