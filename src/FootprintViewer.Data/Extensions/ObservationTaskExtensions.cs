using FootprintViewer.Data.Models;

namespace FootprintViewer.Data.Extensions;

public static class ObservationTaskExtensions
{
    public static Footprint ToFootprint(this ObservationTaskResult taskResult)
    {
        return new Footprint()
        {
            Name = taskResult.Name,
            TargetName = taskResult.TargetName,
            Direction = taskResult.Direction,
            Node = taskResult.Node,
            Begin = taskResult.Interval.Begin,
            Duration = taskResult.Interval.Duration,
            SatelliteName = taskResult.SatelliteName,
            Center = taskResult.Geometry.Center,
            Border = taskResult.Geometry.Border
        };
    }
}
