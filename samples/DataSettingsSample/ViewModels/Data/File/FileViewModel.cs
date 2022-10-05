using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DataSettingsSample.ViewModels
{
    public class FileViewModel : ReactiveObject
    {
        public FileViewModel()
        {

        }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public bool IsSelected { get; set; }

        [Reactive]
        public bool IsVerified { get; set; }
    }
}
