using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public abstract class SidePanelTab : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public string Title { get; set; }
    }
}
