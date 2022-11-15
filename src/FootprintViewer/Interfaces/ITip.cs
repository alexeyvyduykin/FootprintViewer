namespace FootprintViewer;

public interface ITip
{
    double X { get; set; }

    double Y { get; set; }

    bool IsVisible { get; set; }

    object? Content { get; set; }
}