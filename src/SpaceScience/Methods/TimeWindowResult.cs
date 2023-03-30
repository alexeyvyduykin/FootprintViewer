namespace SpaceScience;

public class TimeWindowResult
{
    public string Name { get; set; } = null!;

    public double Lat { get; set; }

    public double Lon { get; set; }

    public int Node { get; set; }

    public bool IsLeftSwath { get; set; }

    public double NadirTime { get; set; }

    public double BeginTime { get; set; }

    public double EndTime { get; set; }

    public double NadirU { get; set; }

    public double BeginU { get; set; }

    public double EndU { get; set; }

    public double MinAngle { get; set; }

    public List<List<(double lonDeg, double latDeg)>> Interval { get; set; } = new();

    public List<List<(double lonDeg, double latDeg)>> Direction { get; set; } = new();
}
