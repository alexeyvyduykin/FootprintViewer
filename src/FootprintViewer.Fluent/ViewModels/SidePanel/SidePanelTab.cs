using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.Fluent.ViewModels.SidePanel;

public abstract class SidePanelTabViewModel : ViewModelBase, ISelectorItem
{
    public string? Key { get; set; }
    //() => GetType().Name;

    [Reactive]
    public string? Title { get; set; }

    [Reactive]
    public bool IsActive { get; set; } = false;

    [Reactive]
    public bool IsExpanded { get; set; }
}
