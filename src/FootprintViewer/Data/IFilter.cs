using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.Data
{
    public interface IFilter<T>
    {
        bool Filtering(T value);

        string[]? Names { get; }

        ReactiveCommand<Unit, IFilter<T>> Update { get; }
    }
}
