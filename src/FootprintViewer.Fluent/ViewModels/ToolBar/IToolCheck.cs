using System;
using System.Reactive;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public interface IToolCheck : ITool
{
    IObservable<IToolCheck> Activate { get; }

    IObservable<Unit> Deactivate { get; }

    bool IsCheck { get; set; }
}
