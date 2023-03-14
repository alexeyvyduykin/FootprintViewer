using System;

namespace FootprintViewer.ViewModels;

public interface IToolCheck2 : ITool
{
    IObservable<bool> IsCheckObservable { get; }

    bool IsCheck { get; set; }
}
