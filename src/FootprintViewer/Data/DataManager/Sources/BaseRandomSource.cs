namespace FootprintViewer.Data.DataManager.Sources;

public abstract class BaseRandomSource : BaseSource
{
    public int GenerateCount { get; init; } = 0;
}
