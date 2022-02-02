using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FootprintViewer.ViewModels.ToolBars
{
    public class ToolCheck : ReactiveObject, IToolCheck
    {
        public ToolCheck()
        {
            Check = ReactiveCommand.Create<bool, IToolCheck>(check =>
            {
                IsCheck = check;

                return this;
            });
        }

        [Reactive]
        public bool IsCheck { get; set; }

        public string? Group { get; set; }

        public string? Tooltip { get; set; }

        public string? Title { get; set; }

        public ReactiveCommand<bool, IToolCheck> Check { get; }
    }
}
