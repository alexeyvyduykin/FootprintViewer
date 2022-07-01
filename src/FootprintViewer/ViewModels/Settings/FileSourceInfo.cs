using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface IFileSourceInfo : ISourceInfo
    {
        string? Path { get; set; }

        public string? FilterName { get; set; }

        public string? FilterExtension { get; set; }
    }

    public class FileSourceInfo : ReactiveObject, IFileSourceInfo
    {
        public FileSourceInfo()
        {

        }

        public string? Name => System.IO.Path.GetFileName(Path);

        [Reactive]
        public string? Path { get; set; }

        public string? FilterName { get; set; }

        public string? FilterExtension { get; set; }
    }
}
