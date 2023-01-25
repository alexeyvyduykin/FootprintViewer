using DynamicData;
using FootprintViewer.Data.Models;
using FootprintViewer.ViewModels.TimelinePanel;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using TimeDataViewer.Core;
using TimelineInterval = FootprintViewer.ViewModels.TimelinePanel.Interval;

namespace PlannedScheduleViewerSample.ViewModels;

public partial class MainWindowViewModel
{
    private readonly DateTime _timeOrigin = new(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);

    private void InitImpl(string satName, PlannedScheduleResult result)
    {
        Begin = ToTotalDays(Epoch, _timeOrigin);
        Duration = 1.0;
        PlotModel = CreatePlotModel(satName, result);
    }

    private PlotModel CreatePlotModel(string satName, PlannedScheduleResult result)
    {
        var satelliteNames = result.PlannedSchedules.Keys.ToList();

        var min = result.PlannedSchedules.Values.Select(s => s.Min(t => t.Interval.Begin)).Min();
        var max = result.PlannedSchedules.Values.Select(s => s.Max(t => t.Interval.Begin)).Max();

        Epoch = min.Date;

        BeginScenario = ToTotalDays(Epoch.Date, _timeOrigin) - 1;
        EndScenario = BeginScenario + 3;
         
        var items = result.PlannedSchedules[satName]         
            .Select(s =>
            {
                return CreateInterval(s.TaskName, Epoch, s.Interval.Begin, s.Interval.Duration);
            }).ToList();

        var Labels = result.PlannedSchedules[satName]
            .Take(5)
            .Select(s => new LabelViewModel() { Label = s.TaskName })
            .ToList();

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

        plotModel.Series.Add(CreateSeries(items.Take(5)));

        return plotModel;
    }

    private static Series CreateSeries(IEnumerable<TimelineInterval> intervals)
    {
        return new TimelineSeries()
        {
            BarWidth = 0.5,
            ItemsSource = intervals,
            CategoryField = "Category",
            BeginField = "BeginTime",
            EndField = "EndTime",
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

    private static TimelineInterval CreateInterval(string category, DateTime epoch, DateTime begin, double duration)
    {
        var secs = begin.TimeOfDay.TotalSeconds;

        var date = epoch.Date;

        return new TimelineInterval()
        {
            Category = category,
            BeginTime = date.AddSeconds(secs),
            EndTime = date.AddSeconds(secs + duration)
        };
    }

    private static double ToTotalDays(DateTime value, DateTime timeOrigin)
    {
        return (value - timeOrigin).TotalDays + 1;
    }

    [Reactive]
    public PlotModel? PlotModel { get; set; }

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