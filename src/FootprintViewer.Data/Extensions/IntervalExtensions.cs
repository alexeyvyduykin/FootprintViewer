using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.Extensions;

public static class IntervalExtensions
{
    public static List<Interval> Merge(this List<Interval> intervals)
    {
        var mergedIntervals = new List<Interval>();
        var orderedIntervals = intervals.OrderBy(s => s.Begin).ToList();

        var begin = orderedIntervals.First().Begin;
        var end = orderedIntervals.First().End();

        Interval currentInterval;
        for (int i = 1; i < orderedIntervals.Count; i++)
        {
            currentInterval = orderedIntervals[i];

            if (currentInterval.Begin < end)
            {
                end = currentInterval.End();
            }
            else
            {
                mergedIntervals.Add(new Interval()
                {
                    Begin = begin,
                    Duration = (end - begin).TotalSeconds
                });

                begin = currentInterval.Begin;
                end = currentInterval.End();
            }
        }

        mergedIntervals.Add(new Interval()
        {
            Begin = begin,
            Duration = (end - begin).TotalSeconds
        });

        return mergedIntervals;
    }

    public static DateTime End(this Interval interval)
        => interval.Begin.AddSeconds(interval.Duration);
}
