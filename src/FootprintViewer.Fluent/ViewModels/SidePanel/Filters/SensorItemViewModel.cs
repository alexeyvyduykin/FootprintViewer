using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Filters;

public class SensorItemViewModel : ViewModelBase
{
    [Reactive]
    public string Name { get; set; } = string.Empty;

    [Reactive]
    public bool IsActive { get; set; } = true;
}
