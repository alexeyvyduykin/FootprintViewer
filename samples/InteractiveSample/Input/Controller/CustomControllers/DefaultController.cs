using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteractiveSample.Input.Controller
{
    public class DefaultController : ControllerBase, IMapController
    {
        public DefaultController()
        {
            this.BindMouseDown(MouseButton.Left, MapCommands.Default);
        }
    }
}
