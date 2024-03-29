﻿using Avalonia.Media;
using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using FootprintViewer.Data.Extensions;
using FootprintViewer.Data.Models;
using FootprintViewer.UI.ViewModels.Dialogs;
using FootprintViewer.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TimeDataViewer.Core;

namespace FootprintViewer.UI.ViewModels.Timelines;

public class TimelinesOldViewModel : DialogViewModelBase<object>
{
    private readonly ILocalStorageService _localStorage;
    private readonly DateTime _timeOrigin = new(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    public TimelinesOldViewModel()
    {
        _localStorage = Services.Locator.GetRequiredService<ILocalStorageService>();

        SelectedInterval = ReactiveCommand.Create<object?>(SelectedIntervalImpl, outputScheduler: RxApp.MainThreadScheduler);

        Init = ReactiveCommand.CreateFromTask(InitImpl, outputScheduler: RxApp.MainThreadScheduler);

        var cancelCommandCanExecute = this.WhenAnyValue(x => x.IsDialogOpen).ObserveOn(RxApp.MainThreadScheduler);

        NextCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Normal), cancelCommandCanExecute);

        Observable.StartAsync(InitImpl, RxApp.MainThreadScheduler);
    }

    public override string Title { get => "Planned Schedule timelines"; protected set { } }

    private ReactiveCommand<Unit, Unit> Init { get; }

    public ReactiveCommand<object?, Unit> SelectedInterval { get; }

    private async Task InitImpl()
    {
        Begin = ToTotalDays(Epoch, _timeOrigin);
        Duration = 1.0;
        SeriesBrushes = new[] { Brushes.LightCoral, Brushes.Green, Brushes.Blue, Brushes.Red, Brushes.Yellow };
        PlotModel = await CreatePlotModel();
    }

    private void SelectedIntervalImpl(object? value)
    {
        if (value is TrackerHitResult)
        {

        }
    }

    private async Task<PlotModel> CreatePlotModel()
    {
        var ps = (await _localStorage.GetValuesAsync<PlannedScheduleResult>(DbKeys.PlannedSchedules.ToString())).FirstOrDefault();

        if (ps == null)
        {
            throw new Exception();
        }

        var footprints = ps.PlannedSchedules
            .Where(s => s is ObservationTaskResult)
            .Cast<ObservationTaskResult>()
            .Select(s => s.ToFootprint())
            .ToList();

        var list = footprints.ToList();
        var min = list.Select(s => s.Begin).Min();
        var max = list.Select(s => s.Begin).Max();
        var satellites = list.Select(s => s.SatelliteName!).Distinct().ToList() ?? new List<string>();

        Epoch = min.Date;

        BeginScenario = ToTotalDays(Epoch.Date, _timeOrigin) - 1;
        EndScenario = BeginScenario + 3;

        var dict = new Dictionary<string, IList<TimeDataViewerLite.IntervalInfo>>();
        foreach (var satName in satellites)
        {
            var items = footprints
                .Where(s => Equals(s.SatelliteName, satName))
                .Select(s => CreateInterval(s, Epoch)).ToList();
            dict.Add(satName, items);
        }
        var Labels = satellites.Select(s => new LabelViewModel() { Label = s }).ToList();

        var plotModel = new PlotModel()
        {
            PlotMarginLeft = 0,
            PlotMarginTop = 30,
            PlotMarginRight = 0,
            PlotMarginBottom = 0
        };

        plotModel.Axises.AddRange(new[]
        {
            CreateAxisY(Labels),
            CreateAxisX(Epoch, BeginScenario, EndScenario)
        });

        plotModel.Series.AddRange(dict.Values.Select(s => CreateSeries(s)).ToList());

        return plotModel;
    }

    private static Series CreateSeries(IEnumerable<TimeDataViewerLite.IntervalInfo> intervals)
    {
        return new TimelineSeries()
        {
            BarWidth = 0.5,
            ItemsSource = intervals,
            CategoryField = "Category",
            BeginField = "Begin",
            EndField = "End",
            IsVisible = true,
            TrackerKey = intervals.FirstOrDefault()?.Category ?? string.Empty,
        };
    }

    private static Axis CreateAxisY(IEnumerable<LabelViewModel> labels)
    {
        var axisY = new CategoryAxis()
        {
            Position = AxisPosition.Left,
            AbsoluteMinimum = -0.5,
            AbsoluteMaximum = 4.5,
            IsZoomEnabled = false,
            LabelField = "Label",
            IsTickCentered = false,
            GapWidth = 1.0,
            ItemsSource = labels
        };

        axisY.Labels.Clear();
        axisY.Labels.AddRange(labels.Select(s => s.Label)!);

        return axisY;
    }

    private static Axis CreateAxisX(DateTime epoch, double begin, double end)
    {
        return new DateTimeAxis()
        {
            Position = AxisPosition.Top,
            IntervalType = DateTimeIntervalType.Auto,
            AbsoluteMinimum = begin,
            AbsoluteMaximum = end,
            CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek,
            FirstDayOfWeek = DayOfWeek.Monday,
            MinorIntervalType = DateTimeIntervalType.Auto,
            Minimum = DateTimeAxis.ToDouble(epoch),
            AxisDistance = 0.0,
            AxisTickToLabelDistance = 4.0,
            ExtraGridlines = null,
            IntervalLength = 60.0,
            IsPanEnabled = true,
            IsAxisVisible = true,
            IsZoomEnabled = true,
            Key = null,
            MajorStep = double.NaN,
            MajorTickSize = 7.0,
            MinorStep = double.NaN,
            MinorTickSize = 4.0,
            Maximum = double.NaN,
            MinimumRange = 0.0,
            MaximumRange = double.PositiveInfinity,
            StringFormat = null
        };
    }

    private static TimeDataViewerLite.IntervalInfo CreateInterval(Footprint footprint, DateTime epoch)
    {
        var secs = footprint.Begin.TimeOfDay.TotalSeconds;

        var date = epoch.Date;

        return new TimeDataViewerLite.IntervalInfo()
        {
            Category = footprint.SatelliteName!,
            Begin = date.AddSeconds(secs),
            End = date.AddSeconds(secs + footprint.Duration)
        };
    }

    private static double ToTotalDays(DateTime value, DateTime timeOrigin)
    {
        return (value - timeOrigin).TotalDays + 1;
    }

    [Reactive]
    public PlotModel? PlotModel { get; set; }

    [Reactive]
    public IList<IBrush>? SeriesBrushes { get; set; }

    [Reactive]
    public DateTime Epoch { get; set; }

    [Reactive]
    public double BeginScenario { get; set; }

    [Reactive]
    public double EndScenario { get; set; }

    [Reactive]
    public double Begin { get; set; }

    [Reactive]
    public double Duration { get; set; }
}

