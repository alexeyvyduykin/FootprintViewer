using FootprintViewer.Factories;
using FootprintViewer.Fluent.ViewModels.Navigation;
using ReactiveUI;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class DemoPageViewModel : RoutableViewModel
{
    public DemoPageViewModel()
    {
        EnableBack = true;
        EnableCancel = true;

        NextCommand = ReactiveCommand.Create(OnNext);
    }

    private void OnNext()
    {
        Navigate().Clear();

        var factory = new DevWorkDataFactory();

        var data = factory.CreateDataManager();

        foreach (var (key, sources) in data.GetSources())
        {
            foreach (var source in sources)
            {
                Services.DataManager.RegisterSource(key, source);
            }
        }

        Services.DataManager.UpdateData();
    }

    public override string Title { get => "Add demo Planned Schedule"; protected set { } }
}
