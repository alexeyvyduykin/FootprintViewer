using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.Fluent.ViewModels.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TimeDataViewerLite;
using TimeDataViewerLite.Core;
using TimeDataViewerLite.Core.Style;

namespace FootprintViewer.Fluent.ViewModels.Timelines;

public class TimelinesViewModel : DialogViewModelBase<object>
{
    private readonly IDataManager _dataManager;

    public TimelinesViewModel()
    {
        _dataManager = Services.DataManager;

        Init = ReactiveCommand.CreateFromTask(UpdateAsyncImpl, outputScheduler: RxApp.MainThreadScheduler);

        var cancelCommandCanExecute = this.WhenAnyValue(x => x.IsDialogOpen).ObserveOn(RxApp.MainThreadScheduler);

        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Normal), cancelCommandCanExecute);

        Observable.StartAsync(UpdateAsyncImpl, RxApp.MainThreadScheduler);
    }

    private ReactiveCommand<Unit, Unit> Init { get; }

    [Reactive]
    public PlotModel? PlotModel { get; set; }

    private async Task UpdateAsyncImpl()
    {
        var plannedSchedules = await _dataManager.GetDataAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString());

        var ps = plannedSchedules.First();

        var state = PlotModel?.GetState();

        var seriesInfos = ToSeriesInfo(ps);

        var tasks = ps.PlannedSchedules
            .OrderBy(s => s.Interval.Begin)
            .Select(s => s.TaskName)
            .Distinct()
            .ToList();

        PlotModel = await Observable.Start(() => PlotModelBuilder.Build(tasks, seriesInfos, state),
            RxApp.TaskpoolScheduler);
    }

    private static List<SeriesInfo> ToSeriesInfo(PlannedScheduleResult ps)
    {
        var windows = ps.TaskAvailabilities;

        var dict = new Dictionary<string, List<(ITaskResult taskResult, Interval ival)>>();

        foreach (var taskResult in ps.PlannedSchedules)
        {
            var satName = taskResult.SatelliteName;
            var ival = taskResult.Interval;

            if (dict.ContainsKey(satName) == false)
            {
                dict.Add(satName, new());
            }

            dict[satName].Add((taskResult, ival));
        }

        var seriesInfos = new List<SeriesInfo>();

        foreach (var (satName, i) in dict.Keys.Select((s, i) => (s, i)))
        {
            var list1 = dict[satName];

            var list2 = windows
                .Where(s => Equals(s.SatelliteName, satName))
                .SelectMany(s => s.Windows.Select(t => (taskName: s.TaskName, ival: t)))
                .ToList();

            var series1 = new SeriesInfo()
            {
                Name = $"{satName}_ivals",
                Items = list1
                .Select(s => new IntervalInfo()
                {
                    Begin = s.ival.Begin,
                    End = s.ival.End(),
                    Category = s.taskResult.TaskName,
                    BrushMode = ToBrushMode(s.taskResult)
                }).ToList(),
                Brush = new Brush(Colors.Palette[i]),
                StackGroup = satName
            };

            static BrushMode ToBrushMode(ITaskResult taskResult)
            {
                if (taskResult is CommunicationTaskResult comRes)
                {
                    if (comRes.Type == CommunicationType.Uplink)
                    {
                        return BrushMode.UpLine;
                    }
                    else if (comRes.Type == CommunicationType.Downlink)
                    {
                        return BrushMode.DownLine;
                    }
                }

                return BrushMode.Solid;
            }

            var series2 = new SeriesInfo()
            {
                Name = $"{satName}_windows",
                Items = list2
                .Select(s => new IntervalInfo()
                {
                    Begin = s.ival.Begin,
                    End = s.ival.End(),
                    Category = s.taskName,
                    BrushMode = BrushMode.Solid
                }).ToList(),
                Brush = new Brush(Colors.Palette[i], 0.25),
                StackGroup = satName
            };

            seriesInfos.Add(series2);
            seriesInfos.Add(series1);
        }

        return seriesInfos;
    }
}
