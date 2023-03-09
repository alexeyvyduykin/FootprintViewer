using FootprintViewer.Data.Models;

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
}
