using FootprintViewer.Data.Sources;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public interface IFileSourceViewModel : ISourceViewModel
    {
        string? Path { get; set; }

        public string? FilterName { get; set; }

        public string? FilterExtension { get; set; }
    }

    public class FileSourceViewModel : ReactiveObject, IFileSourceViewModel
    {
        public FileSourceViewModel()
        {

        }

        public FileSourceViewModel(IFileSource fileSource) : this()
        {
            Path = fileSource.Path;
        }

        public string? Name => System.IO.Path.GetFileName(Path);

        [Reactive]
        public string? Path { get; set; }

        public string? FilterName { get; set; }

        public string? FilterExtension { get; set; }
    }
}
