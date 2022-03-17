using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels
{
    public abstract class SidePanelTab : ReactiveObject
    {
        [Reactive]
        public string? Title { get; set; }

        [Reactive]
        public bool IsActive { get; set; } = false;

        [Reactive]
        public bool IsExpanded { get; set; }
    }
}
