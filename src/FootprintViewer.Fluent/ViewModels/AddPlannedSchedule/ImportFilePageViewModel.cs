using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.Models;
using FootprintViewer.Fluent.ViewModels.Navigation;
using FootprintViewer.Helpers;
using FootprintViewer.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Concurrency;

namespace FootprintViewer.Fluent.ViewModels.AddPlannedSchedule;

public class ImportFilePageViewModel : RoutableViewModel
{
    private readonly string _filePath;

    public ImportFilePageViewModel(string filePath)
    {
        _filePath = filePath;

        EnableBack = true;
        EnableCancel = true;

        var nextCommandCanExecute = this.WhenAnyValue(s => s.IsVerified);

        NextCommand = ReactiveCommand.Create(OnNext, nextCommandCanExecute);

        RxApp.MainThreadScheduler.Schedule(TimeSpan.FromSeconds(1), async () =>
        {
            IsVerified = await JsonHelpers.VerifiedAsync<PlannedScheduleResult>(filePath);

            IsChecked = false;
        });
    }

    public override string Title { get => "Import Planned Schedule from file"; protected set { } }

    [Reactive]
    public bool IsVerified { get; set; }

    [Reactive]
    public bool IsChecked { get; set; } = true;

    private void OnNext()
    {
        Navigate().Clear();

        Services.DataManager.UnregisterSources(DbKeys.PlannedSchedules.ToString());

        foreach (var (key, source) in Global.CreateSources(_filePath))
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
        config.PlannedScheduleState = PlannedScheduleState.JsonFile;

        config.LastPlannedScheduleJsonFile = _filePath;
    }
}