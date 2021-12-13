using ReactiveUI;
using System;

namespace FootprintViewer.ViewModels
{
    public class MapListener : ReactiveObject
    {
        public MapListener()
        {

        }

        public event EventHandler? LeftClickOnMap;

        public void LeftClick(string name)
        {
            LeftClickOnMap?.Invoke(name, EventArgs.Empty);
        }
    }
}
