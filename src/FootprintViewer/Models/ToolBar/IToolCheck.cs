using ReactiveUI;

namespace FootprintViewer.Models
{
    public interface IToolCheck : ITool
    {
        string? Group { get; set; }

        bool IsCheck { get; set; }

        ReactiveCommand<bool, IToolCheck> Check { get; }
    }
}
