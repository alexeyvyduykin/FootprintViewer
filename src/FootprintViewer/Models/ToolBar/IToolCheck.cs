using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.Models
{
    public interface IToolCheck : ITool
    {
        string? Group { get; set; }

        bool IsCheck { get; set; }

        ReactiveCommand<Unit, IToolCheck> BeforeActivate { get; }

        ReactiveCommand<Unit, IToolCheck> Activate { get; }

        ReactiveCommand<Unit, IToolCheck> Deactivate { get; }
    }
}
