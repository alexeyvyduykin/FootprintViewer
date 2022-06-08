namespace FootprintViewer.ViewModels.Settings
{
    public interface IFileSourceInfo : ISourceInfo
    {
        string? Path { get; }
    }

    public class FileSourceInfo : IFileSourceInfo
    {
        public FileSourceInfo()
        {

        }

        public string? Name => System.IO.Path.GetFileName(Path);

        public string? Path { get; set; }
    }
}
