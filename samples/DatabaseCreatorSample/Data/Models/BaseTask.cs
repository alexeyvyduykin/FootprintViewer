using System;

namespace DatabaseCreatorSample.Data.Models;

public abstract class BaseTask
{
    public string? Name { get; set; }

    public string? SatelliteName { get; set; }

    public int Node { get; set; }

    public DateTime ActiveBeginTime { get; set; }

    public double ActiveDuration { get; set; }

    public DateTime? AvailableBeginTime { get; set; }

    public double? AvailableDuration { get; set; }
}
