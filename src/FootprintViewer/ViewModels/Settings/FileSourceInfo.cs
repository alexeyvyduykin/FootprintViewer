using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    public interface IFileSourceInfo : ISourceInfo
    {
        string? Path { get; set; }

        public string? FilterName { get; set; }

        public string? FilterExtension { get; set; }
    }

    [DataContract]
    public class FileSourceInfo : ReactiveObject, IFileSourceInfo
    {
        public FileSourceInfo()
        {

        }

        public string? Name => System.IO.Path.GetFileName(Path);

        [DataMember]
        [Reactive]
        public string? Path { get; set; }

        [DataMember]
        public string? FilterName { get; set; }

        [DataMember]
        public string? FilterExtension { get; set; }
    }
}
