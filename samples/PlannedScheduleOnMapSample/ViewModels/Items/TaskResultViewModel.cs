using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using System;

namespace PlannedScheduleOnMapSample.ViewModels.Items;

public class TaskResultViewModel : ViewModelBase
{
    private readonly static Random _random = new();

    public TaskResultViewModel() : this(CreateObservation("ObservationTask0063", CreateRandom())) { }

    public TaskResultViewModel(ITaskResult taskResult)
    {
        Model = (ObservationTaskResult)taskResult;

        SatelliteName = taskResult.SatelliteName;
        TaskName = taskResult.TaskName;
        Begin = taskResult.Interval.Begin;
        Duration = taskResult.Interval.Duration;
    }

    public string TaskName { get; set; }

    public string SatelliteName { get; set; }

    public DateTime Begin { get; set; }

    public double Duration { get; set; }

    public ObservationTaskResult Model { get; set; }

    //public List<Interval>? Windows { get; set; }

    //public Interval? Transition { get; set; }

    public static ObservationTaskResult CreateObservation(string taskName, Footprint footprint)
    {
        var begin = footprint.Begin;
        var duration = footprint.Duration;

        var taskResult = new ObservationTaskResult()
        {
            Name = footprint.Name,
            TargetName = footprint.TargetName,
            TaskName = taskName,
            SatelliteName = footprint.SatelliteName ?? "SatelliteDefault",
            Interval = new Interval { Begin = begin, Duration = duration },
            Node = footprint.Node,
            Direction = footprint.Direction,
            Geometry = new FootprintGeometry { Center = footprint.Center, Border = footprint.Border },
            Transition = null
        };

        return taskResult;
    }

    public static Footprint CreateRandom()
    {
        return new Footprint()
        {
            Name = $"Footprint{_random.Next(1, 101):000}",
            SatelliteName = $"Satellite{_random.Next(1, 10):00}",
            Center = new Point(_random.Next(-180, 180), _random.Next(-90, 90)),
            Begin = DateTime.Now,
            Duration = _random.Next(20, 40),
            Node = _random.Next(1, 16),
            Direction = (SwathDirection)_random.Next(0, 2),
        };
    }

    public static Footprint Create(ObservationTaskResult taskResult)
    {
        var node = taskResult.Node;
        var begin = taskResult.Interval.Begin;
        var duration = taskResult.Interval.Duration;
        var direction = taskResult.Direction;
        var satName = taskResult.SatelliteName;
        var targetName = taskResult.TargetName;
        var taskName = taskResult.TaskName;
        var center = taskResult.Geometry.Center;
        var border = taskResult.Geometry.Border;

        return new Footprint()
        {
            Name = $"Footprint_{taskName}",
            TargetName = targetName,
            SatelliteName = satName,
            Center = center,
            Border = border,
            Begin = begin.AddSeconds(duration / 2.0),
            Duration = duration,
            Node = node + 1,
            Direction = direction,
        };
    }
}
