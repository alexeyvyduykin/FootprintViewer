using FootprintViewer.Fluent.ViewModels.Navigation;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class ImportFilePageViewModel : RoutableViewModel
{
    public ImportFilePageViewModel(string filePath)
    {
        EnableBack = true;
        EnableCancel = true;
    }

    public override string Title { get => "Import Planned Schedule from file"; protected set { } }
}
