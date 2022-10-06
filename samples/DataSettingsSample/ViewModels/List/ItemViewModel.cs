using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace DataSettingsSample.ViewModels
{
    public class ItemViewModel : ReactiveObject
    {
        [Reactive]
        public string? Name { get; set; }
    }
}
