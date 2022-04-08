using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public abstract class SidePanelTab : ReactiveObject, ISelectorItem
    {
        public string GetKey() => GetType().Name;

        [Reactive]
        public string? Title { get; set; }

        [Reactive]
        public bool IsActive { get; set; } = false;

        [Reactive]
        public bool IsExpanded { get; set; }
    }
}
