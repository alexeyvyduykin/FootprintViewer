using FootprintViewer.Data.DbContexts;
using FootprintViewer.UI.Models;
using FootprintViewer.UI.ViewModels.Navigation;
using FootprintViewer.Logging;
using FootprintViewer.Services;
using ReactiveUI;
using System.Reactive.Concurrency;

namespace FootprintViewer.UI.ViewModels.AddPlannedSchedule;

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

        var localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        localStorage.UnregisterSources(DbKeys.PlannedSchedules.ToString());

        foreach (var (key, source) in Global.CreateDemoSources())
        {
            localStorage.RegisterSource(key, source);
        }

        localStorage.UpdateData();

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
