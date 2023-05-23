using FootprintViewer.Data;
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

        var dataManager = Services.Locator.GetRequiredService<IDataManager>();

        dataManager.UnregisterSources(DbKeys.PlannedSchedules.ToString());

        foreach (var (key, source) in Global.CreateDemoSources())
        {
            dataManager.RegisterSource(key, source);
        }

        dataManager.UpdateData();

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
