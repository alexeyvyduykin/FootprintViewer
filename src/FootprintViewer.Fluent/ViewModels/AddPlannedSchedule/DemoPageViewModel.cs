using FootprintViewer.Data.DbContexts;
using FootprintViewer.Fluent.Models;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Logging;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class DemoPageViewModel : RoutableViewModel
{
    public DemoPageViewModel()
    {
        EnableBack = true;
        EnableCancel = true;

        NextCommand = ReactiveCommand.Create(OnNext);
    }

    public override string Title { get => "Add demo Planned Schedule"; protected set { } }

    private void OnNext()
    {
        Navigate().Clear();

        Services.DataManager.UnregisterSources(DbKeys.PlannedSchedules.ToString());

        foreach (var (key, source) in Global.CreateDemoSources())
        {
            Services.DataManager.RegisterSource(key, source);
        }

        Services.DataManager.UpdateData();

        Save();
    }

    private void Save()
    {
        var config = new Config(Services.Config.FilePath);

        RxApp.MainThreadScheduler.Schedule(
            () =>
            {
                try
                {
                    config.LoadFile();
                    EditConfigOnSave(config);
                    config.ToFile();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex);
                }
            });
    }

    private void EditConfigOnSave(Config config)
    {
        config.PlannedScheduleState = PlannedScheduleState.Demo;
    }
}
