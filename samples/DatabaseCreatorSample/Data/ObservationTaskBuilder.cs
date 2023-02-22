using FootprintViewer.Data;
using FootprintViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCreatorSample.Data;

public static class ObservationTaskBuilder
{
    private static readonly Random _random = new();

    public static Dictionary<string, List<ITaskResult>> Create(IList<ITask> tasks, IList<Footprint>? footprints)
    {
        var dict = new Dictionary<string, List<ITaskResult>>();

        if (footprints == null)
        {
            return dict;
        }

        foreach (var item in tasks.Where(s => s is ObservationTask).Cast<ObservationTask>())
        {
            var list = footprints.Where(s => Equals(s.TargetName, item.TargetName)).ToList();

            foreach (var footprint in list)
            {
                var satelliteName = footprint.SatelliteName!;
                var begin = footprint.Begin;
                var duration = footprint.Duration;

                var windowDuration = duration * (_random.Next(30, 51) / 10.0);

                var windowBeginSec = _random.Next(0, (int)(windowDuration - duration) + 1);

                var windowBegin = begin.AddSeconds(-windowBeginSec);

                var task = new ObservationTaskResult()
                {
                    TaskName = item.Name,
                    Interval = new Interval { Begin = begin, Duration = duration },
                    Footprint = new FootprintFrame { Center = footprint.Center!, Points = footprint.Points! },
                    Windows = new[] { new Interval { Begin = windowBegin, Duration = windowDuration } }.ToList(),
                    Transition = null
                };

                if (dict.ContainsKey(satelliteName) == false)
                {
                    dict.Add(satelliteName, new List<ITaskResult>());
                }

                dict[satelliteName].Add(task);
            }
        }

        return dict;
    }
}
