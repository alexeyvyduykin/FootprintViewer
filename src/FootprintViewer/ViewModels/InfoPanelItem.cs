using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public class InfoPanelItem : ReactiveObject
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public string CommandTitle { get; set; }

        public ICommand Command { get; set; }
    }
}
