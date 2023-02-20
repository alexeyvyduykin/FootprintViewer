using DatabaseCreatorSample.Data.Models;
using FootprintViewer.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data;

public static class ObservationTaskBuilder
{
    private static readonly Random _random = new();

    public static IList<ObservationTask> Create(IList<Footprint>? footprints)
    {
        var tasks = new List<ObservationTask>();

        if (footprints == null)
        {
            return tasks;
        }

        foreach (var (item, i) in footprints.Select((s, index) => (s, index)))
        {
            var begin = item.Begin;
            var duration = item.Duration;

            var windowDuration = duration * (_random.Next(30, 51) / 10.0);

            var windowBeginSec = _random.Next(0, (int)(windowDuration - duration) + 1);

            var windowBegin = begin.AddSeconds(-windowBeginSec);

            var task = new ObservationTask()
            {
                Name = $"ObservationTask{(i + 1):0000}",
                SatelliteName = item.SatelliteName,
                Node = item.Node,
                ActiveBeginTime = begin,
                ActiveDuration = duration,
                AvailableBeginTime = windowBegin,
                AvailableDuration = windowDuration,
                FootprintName = item.Name,
                GroundTargetName = item.TargetName,
                Center = item.Center,
                Points = item.Points,
                Direction = item.Direction,
            };

            tasks.Add(task);
        }

        return tasks;
    }
}
