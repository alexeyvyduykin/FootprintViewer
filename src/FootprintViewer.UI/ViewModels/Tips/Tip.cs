using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.UI.ViewModels.Tips;

public class Tip : ViewModelBase, ITip
{
    [Reactive]
    public double X { get; set; }

    [Reactive]
    public double Y { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public object? Content { get; set; }
}
