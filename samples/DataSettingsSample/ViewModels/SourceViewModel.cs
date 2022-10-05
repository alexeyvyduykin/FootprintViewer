using DataSettingsSample.ViewModels.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DataSettingsSample.ViewModels
{
    public class SourceViewModel : ReactiveObject, ISourceViewModel
    {
        public SourceViewModel()
        {

        }

        [Reactive]
        public string? Name { get; set; }
    }
}
