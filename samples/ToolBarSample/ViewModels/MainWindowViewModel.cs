using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ToolBarSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ToolBar = new ToolBarViewModel();
        }

        [Reactive]
        public ToolBarViewModel ToolBar { get; set; }
    }
}
