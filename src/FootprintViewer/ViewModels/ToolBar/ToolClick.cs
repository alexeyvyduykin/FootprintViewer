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

        public string GetKey() => (string?)Tag ?? string.Empty;

        public object? Tag { get; set; }

        public ReactiveCommand<Unit, IToolClick> Click { get; }
    }
}
