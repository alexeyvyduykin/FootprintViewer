using System;
using System.Reactive;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public interface IToolCheck : ITool
{
    IObservable<IToolCheck> Activate { get; }

    IObservable<Unit> Deactivate { get; }

    bool IsCheck { get; set; }
}
