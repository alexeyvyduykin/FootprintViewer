using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using NetTopologySuite.Geometries;
using SpaceScience;
using System.Reactive.Linq;

namespace JsonDataBuilderSample.Models;

public static class TaskResultBuilder
{
    public static List<ObservationTaskResult> CreateObservations(IList<ObservationTask> tasks, IList<Satellite> satellites, IList<(string satName, IList<TimeWindowResult> windows)> windows)
    {
        int durationMin = 30;// 10;
        int durationMax = 45;// 30;

        var random = new Random();

        var listTaskResults = new List<ObservationTaskResult>();

        int index = 0;

        foreach (var task in tasks)
        {
            var taskName = task.Name;
            var gtName = task.GroundTargetName;

            var res = windows.Select(s => (s.satName, windows: s.windows.Where(t => Equals(t.Name, gtName)).ToList())).ToList();

            var i = random.Next(0, res.Count);

            if (res[i].windows.Count == 0)
            {
                continue;
            }

            var j = random.Next(0, res[i].windows.Count);

            var satName = res[i].satName;

            var sat = satellites.Where(s => Equals(s.Name, satName)).Single();

            var period = sat.Period;

            var epoch = sat.Epoch;

            var selectRes = res[i].windows[j];

            var duration = random.Next(durationMin, durationMax + 1);
            var timeVisible = selectRes.NadirTime;
            var begin = timeVisible - duration / 2.0;

            var lonDeg = selectRes.Lon;
            var latDeg = selectRes.Lat;

            var node = selectRes.Node;
            var direction = (selectRes.IsLeftSwath == true) ? SwathDirection.Left : SwathDirection.Right;

            var taskResult = new ObservationTaskResult()
            {
                Name = $"Observation{++index:0000}",
                TargetName = gtName,
                TaskName = taskName,
                Node = node,
                Direction = direction,
                SatelliteName = satName,
                Interval = new Interval
                {
                    Begin = epoch.AddSeconds(begin + node * period),
                    Duration = duration
                },
                Geometry = new FootprintGeometry
                {
                    Center = new Point(lonDeg, latDeg),
                    Border = FootprintBuilder.CreateFootprintBorder(lonDeg, latDeg)
                },
                Transition = null
            };

            listTaskResults.Add(taskResult);
        }

        return listTaskResults;
    }
}
