using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.Models
{
    public interface IToolClick : ITool
    {
        ReactiveCommand<Unit, IToolClick> Click { get; }
    }
}
