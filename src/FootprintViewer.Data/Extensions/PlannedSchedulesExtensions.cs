using FootprintViewer.Data.Models;
using SpaceScience;
using SpaceScience.Extensions;
using SpaceScience.Model;

namespace FootprintViewer.Data.Extensions;

public static class PlannedSchedulesExtensions
{
    public static DateTime MinDateTime(this PlannedScheduleResult ps)
    {
        return ps.PlannedSchedules.Min(s => s.Interval.Begin);
    }

    public static DateTime MaxDateTime(this PlannedScheduleResult ps)
    {
        return ps.PlannedSchedules.Max(s => End(s.Interval));

        static DateTime End(Interval ival) => ival.Begin.AddSeconds(ival.Duration);
    }

    public static IList<ObservationTaskResult> GetObservations(this PlannedScheduleResult ps)
    {
        return ps.PlannedSchedules
            .Where(s => s is ObservationTaskResult)
            .Cast<ObservationTaskResult>()
            .ToList();
    }

    public static IList<CommunicationTaskResult> GetCommunications(this PlannedScheduleResult ps)
    {
        return ps.PlannedSchedules
            .Where(s => s is CommunicationTaskResult)
            .Cast<CommunicationTaskResult>()
            .ToList();
    }

    public static Dictionary<string, Dictionary<int, List<List<(double lonDeg, double latDeg)>>>> BuildObservableIntervals(this PlannedScheduleResult ps)
    {
        var dict = new Dictionary<string, Dictionary<int, List<List<(double lonDeg, double latDeg)>>>>();

        foreach (var satellite in ps.Satellites)
        {
            var nodes = satellite.NodesOnDay();
            var name = satellite.Name;

            var dict2 = new Dictionary<int, List<List<(double lonDeg, double latDeg)>>>();

            for (int i = 0; i < nodes; i++)
            {
                var observationTasks = ps.PlannedSchedules
                     .Where(s => s is ObservationTaskResult)
                     .Cast<ObservationTaskResult>()
                     .Where(s => s.Node == i)
                     .Where(s => Equals(s.SatelliteName, name));

                var orbit = satellite.ToOrbit();
                var epoch = satellite.Epoch;
                var period = satellite.Period;

                dict2.Add(i, new());

                foreach (var taskResult in observationTasks)
                {
                    var begin = taskResult.Interval.Begin;
                    var duration = taskResult.Interval.Duration;

                    var track = new GroundTrack(orbit);
                    var begin0 = begin.AddSeconds(-period * i);

                    var t0 = (begin0 - epoch).TotalSeconds;
                    var t1 = t0 + duration;

                    track.CalculateTrackOnTimeInterval(t0, t1, 2);

                    var res = track.GetTrack(i, duration);

                    var trackLine = LonSplitters.Default.Split(res);

                    dict2[i].AddRange(trackLine);
                }
            }

            dict.Add(name, dict2);
        }

        return dict;
    }
}
