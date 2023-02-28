using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Builders;

public static class RandomModelBuilder
{
    private static readonly Random _random = new();

    public static FootprintPreview BuildFootprintPreview()
    {
        var names = new[] { "02-65-lr_2000-3857-lite", "36-65-ur_2000-3857-lite", "38-50-ll_3857-lite", "38-50-lr_3857-lite", "38-50-ul_3857-lite", "38-50-ur_3857-lite", "41-55-ul_2000-3857-lite", "44-70-ur_2000-3857-lite" };
        var satellites = new[] { "Satellite1", "Satellite2", "Satellite3" };

        var name = names[_random.Next(0, names.Length)].Replace("lite", "").Replace("2000", "").Replace("3857", "").Replace("_", "").Replace("-", "");
        var date = DateTime.UtcNow;

        var unitBitmap = new SKBitmap(1, 1);
        unitBitmap.SetPixel(0, 0, SKColors.White);

        return new FootprintPreview()
        {
            Name = name.ToUpper(),
            Date = date.Date.ToShortDateString(),
            SatelliteName = satellites[_random.Next(0, satellites.Length)],
            SunElevation = _random.Next(0, 91),
            CloudCoverFull = _random.Next(0, 101),
            TileNumber = name.ToUpper(),
            Image = SKImage.FromBitmap(unitBitmap)
        };
    }

    public static Footprint BuildFootprint()
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

    public static GroundStation BuildGroundStation()
    {
        return new GroundStation()
        {
            Name = $"London",
            Center = new Point(-0.118092, 51.509865),
            Angles = new double[] { 12, 18, 22, 26, 30 },
        };
    }

    public static GroundTarget BuildGroundTarget()
    {
        var type = (GroundTargetType)_random.Next(0, 3);

        return new GroundTarget()
        {
            Name = $"GroundTarget{_random.Next(1, 101):000}",
            Type = type,
        };
    }

    public static Satellite BuildSatellite()
    {
        return new Satellite()
        {
            Name = $"Satellite{_random.Next(1, 10):00}",
            Semiaxis = 6945.03,
            Eccentricity = 0.0,
            InclinationDeg = 97.65,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = 0.0,
            RightAscensionAscendingNodeDeg = 0.0,
            Period = 5760.0,
            Epoch = DateTime.Now,
            InnerHalfAngleDeg = 32,
            OuterHalfAngleDeg = 48
        };
    }

    public static UserGeometry BuildUserGeometry()
    {
        return new UserGeometry()
        {
            Name = $"UserGeometry{_random.Next(1, 101):000}",
            Type = (UserGeometryType)_random.Next(0, 4),
        };
    }

    public static PlannedScheduleResult BuildPlannedSchedule()
    {
        var count = 10;

        var footprints = Enumerable.Range(0, count).Select(_ => BuildFootprint()).ToList();

        var tasks = footprints.Select((s, i) => (ITask)new ObservationTask() { Name = $"ObservationTask{i + 1}", GroundTargetName = s.TargetName! }).ToList();

        var list = tasks.Select((s, i) => CreateObservationTaskResult(s.Name, footprints[i])).ToList();

        return new PlannedScheduleResult()
        {
            Name = "PlannedSchedule01",
            DateTime = DateTime.Now,
            Tasks = tasks,
            PlannedSchedules = list
        };
    }

    public static List<ITaskResult> Create(IList<ITask> tasks, IList<Footprint>? footprints)
    {
        if (footprints == null)
        {
            return new();
        }

        return tasks
            .Where(s => s is ObservationTask)
            .Cast<ObservationTask>()
            .SelectMany(s =>
                footprints
                    .Where(f => Equals(f.TargetName, s.GroundTargetName))
                    .Select(f => CreateObservationTaskResult(s.Name, f)))
            .ToList();

        //foreach (var item in tasks.Where(s => s is ObservationTask).Cast<ObservationTask>())
        //{
        //    var list = footprints.Where(s => Equals(s.TargetName, item.TargetName)).ToList();

        //    foreach (var footprint in list)
        //    {
        //        var satelliteName = footprint.SatelliteName!;

        //        var task = CreateObservationTaskResult(item.Name, footprint);

        //        result.Add(task);
        //    }
        //}

        //return result;
    }

    public static ITaskResult CreateObservationTaskResult(string taskName, Footprint footprint)
    {
        var begin = footprint.Begin;
        var duration = footprint.Duration;

        var windowDuration = duration * (_random.Next(30, 51) / 10.0);

        var windowBeginSec = _random.Next(0, (int)(windowDuration - duration) + 1);

        var windowBegin = begin.AddSeconds(-windowBeginSec);

        return new ObservationTaskResult()
        {
            TaskName = taskName,
            SatelliteName = footprint.SatelliteName ?? "SatelliteDefault",
            Interval = new Interval { Begin = begin, Duration = duration },
            Footprint = new FootprintFrame { Center = footprint.Center!, Points = footprint.Points! },
            Windows = new[] { new Interval { Begin = windowBegin, Duration = windowDuration } }.ToList(),
            Transition = null
        };
    }
}
