using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public class Tool : ReactiveObject
    {
        [Reactive]
        public bool IsActive { get; set; }

        [Reactive]
        public string Title { get; set; }

        public ICommand Command { get; set; }
    }
}
