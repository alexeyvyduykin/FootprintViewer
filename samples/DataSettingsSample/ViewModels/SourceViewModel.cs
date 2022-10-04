using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DataSettingsSample.ViewModels
{
    public class SourceViewModel : ReactiveObject
    {
        public SourceViewModel()
        {

        }

        [Reactive]
        public string? Name { get; set; }
    }
}
