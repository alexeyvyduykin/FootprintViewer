using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ToolBarSample.ViewModels;

public class ToolCheck : ViewModelBase
{
    public ToolCheck()
    {

    }

    [Reactive]
    public bool IsCheck { get; set; } = false;

    public object? Tag { get; set; }
}

