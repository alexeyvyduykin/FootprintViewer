using DynamicData;
using FootprintViewer.Data.Models;
using JsonDataBuilderSample;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

namespace PlannedScheduleViewerSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly SourceList<TaskViewModel> _tasks = new();
    private readonly SourceList<SatelliteViewModel> _satellites = new();
    private readonly SourceList<ScheduleItemViewModel> _schedules = new();

    private readonly ReadOnlyObservableCollection<TaskViewModel> _taskItems;
    private readonly ReadOnlyObservableCollection<SatelliteViewModel> _satelliteItems;
    private readonly ReadOnlyObservableCollection<ScheduleItemViewModel> _scheduleItems;

    private readonly PlannedScheduleResult _result;

    public MainWindowViewModel()
    {
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        var plannedSchedulePath = Path.GetFullPath(Path.Combine(root, @"..\..\..\..\JsonDataBuilderSample\Output", "PlannedSchedule.json"));

        _result = (PlannedScheduleResult)JsonHelper.DeserializeFromFile<PlannedScheduleResult>(plannedSchedulePath, new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        })!;

        _tasks
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _taskItems)
            .Subscribe();

        _satellites
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _satelliteItems)
            .Subscribe();

        _schedules
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _scheduleItems)
            .Subscribe();

        LoadTasks(_result!);
        LoadSatellites(_result!);

        SatelliteCheck = ReactiveCommand.Create<SatelliteViewModel>(SatelliteCheckImpl, outputScheduler: RxApp.MainThreadScheduler);

        this.WhenAnyValue(s => s.CurrentSatellite)
            .Subscribe(s => LoadShedules(s!));
    }

    public ReactiveCommand<SatelliteViewModel, Unit> SatelliteCheck { get; }

    private void SatelliteCheckImpl(SatelliteViewModel parameter)
    {
        CurrentSatellite = parameter;
    }

    private void LoadTasks(PlannedScheduleResult result)
    {
        var list = result.Tasks.Select(s => new TaskViewModel(s)).ToList();

        _tasks.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    private void LoadSatellites(PlannedScheduleResult result)
    {
        var list = result.PlannedSchedules.Keys.Select((s, index) => new SatelliteViewModel()
        {
            Name = s,
            IsCheck = (index == 0)
        });

        CurrentSatellite = list.FirstOrDefault()!;

        _satellites.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    private void LoadShedules(SatelliteViewModel satellite)
    {
        var satelliteName = satellite.Name!;

        var list = _result.PlannedSchedules[satelliteName].Items
            .Select(s => new ScheduleItemViewModel(s))
            .ToList();

        _schedules.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    public ReadOnlyObservableCollection<TaskViewModel> Tasks => _taskItems;

    public ReadOnlyObservableCollection<SatelliteViewModel> Satellites => _satelliteItems;

    public ReadOnlyObservableCollection<ScheduleItemViewModel> ScheduleItems => _scheduleItems;

    [Reactive]
    public SatelliteViewModel? CurrentSatellite { get; set; }
}
