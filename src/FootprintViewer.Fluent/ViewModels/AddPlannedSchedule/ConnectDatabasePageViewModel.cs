using FootprintViewer.Fluent.ViewModels.Navigation;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

internal class ConnectDatabasePageViewModel : RoutableViewModel
{
    public ConnectDatabasePageViewModel()
    {
        EnableBack = true;
        EnableCancel = true;
    }

    public override string Title { get => "Connect database"; protected set { } }
}
