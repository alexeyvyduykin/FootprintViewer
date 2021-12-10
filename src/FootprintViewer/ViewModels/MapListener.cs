using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class MapListener : ReactiveObject
    {
        public MapListener()
        {

        }

        public event EventHandler? ClickOnMap;

        public void Click(string name)
        {
            ClickOnMap?.Invoke(name, EventArgs.Empty);
        }
    }
}
