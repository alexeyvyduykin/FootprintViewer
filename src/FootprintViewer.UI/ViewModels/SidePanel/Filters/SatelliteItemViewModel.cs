using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.UI.ViewModels.SidePanel.Filters;

public class SatelliteItemViewModel : ViewModelBase
{
    public string? Name { get; set; }

    [Reactive]
    public bool IsActive { get; set; } = true;
}
