namespace FootprintViewer.Data.RandomSources;

public abstract class BaseRandomSource : BaseSource
{
    public int GenerateCount { get; init; } = 0;
}
