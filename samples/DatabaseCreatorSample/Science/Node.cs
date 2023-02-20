using System.Collections.Generic;

namespace DatabaseCreatorSample.Science;

public struct NodeQuarter
{
    public int Quart { get; set; }
    public double TimeBegin { get; set; }
    public double TimeEnd { get; set; }
}


public class Node
{
    public int Value { get; set; }
    public List<NodeQuarter> Quarts { get; protected set; } = new List<NodeQuarter>();
}
