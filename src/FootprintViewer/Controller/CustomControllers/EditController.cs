using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer
{
    public class EditController : ControllerBase, IMapController
    {
        public EditController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Editing);    
        }
    }
}
