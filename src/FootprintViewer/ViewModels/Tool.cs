using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public class Tool : ReactiveObject
    {
        [Reactive]
        public bool IsActive { get; set; }

        [Reactive]
        public string? Tooltip { get; set; }

        [Reactive]
        public string? Title { get; set; }

        public ReactiveCommand<Unit, Unit>? Command { get; set; }

        [Reactive]
        public object? Content { get; set; }
    }
}
