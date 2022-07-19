namespace FootprintViewer.Data.Sources
{
    public abstract class BaseFileSource : IFileSource
    {
        public string Path { get; init; } = string.Empty;
    }
}
