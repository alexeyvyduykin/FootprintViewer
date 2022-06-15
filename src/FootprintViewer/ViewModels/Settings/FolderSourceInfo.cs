using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.IO;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    public interface IFolderSourceInfo : ISourceInfo
    {
        string? Directory { get; set; }

        string? SearchPattern { get; set; }
    }

    [DataContract]
    public class FolderSourceInfo : ReactiveObject, IFolderSourceInfo
    {
        public FolderSourceInfo()
        {

        }

        private static string GetName(string directory)
        {
            string[] res = directory.Split(Path.DirectorySeparatorChar);

            if (res.Length > 1)
            {
                return $"{res[^2]}/{res[^1]}";
            }

            if (res.Length > 0)
            {
                return res[^1];
            }

            return string.Empty;
        }

        public string? Name => Directory != null ? GetName(Directory) : string.Empty;

        [DataMember]
        [Reactive]
        public string? Directory { get; set; }

        [DataMember]
        public string? SearchPattern { get; set; }
    }
}
