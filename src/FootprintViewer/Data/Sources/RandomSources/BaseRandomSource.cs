namespace FootprintViewer.Data.Sources
{
    public abstract class BaseRandomSource : IRandomSource
    {
        public string Name { get; init; } = string.Empty;
    }
}
