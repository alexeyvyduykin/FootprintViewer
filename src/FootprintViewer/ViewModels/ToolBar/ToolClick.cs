using FootprintViewer.Models;
using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public class ToolClick : ReactiveObject, IToolClick
    {
        public ToolClick()
        {
            Click = ReactiveCommand.Create<Unit, IToolClick>(_ => this);
        }

        public string? Tooltip { get; set; }

        public string? Title { get; set; }

        public ReactiveCommand<Unit, IToolClick> Click { get; }
    }
}
