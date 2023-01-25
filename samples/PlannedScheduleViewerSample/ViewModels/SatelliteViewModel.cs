using ReactiveUI.Fody.Helpers;

namespace PlannedScheduleViewerSample.ViewModels;

public class SatelliteViewModel : ViewModelBase
{
    [Reactive]
    public string? Name { get; set; }

    [Reactive]
    public bool IsCheck { get; set; }
}
