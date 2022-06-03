using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
    public interface IFilter<T>
    {
        bool Filtering(T value);

        string[]? Names { get; }

        ReactiveCommand<Unit, IFilter<T>> Update { get; }
    }

    public abstract class ViewerListFilter<T> : ReactiveObject, IFilter<T> where T : IViewerItem
    {
        public ViewerListFilter()
        {
            Update = ReactiveCommand.Create<IFilter<T>>(() => this);
        }

        public abstract string[]? Names { get; }

        public abstract bool Filtering(T value);

        public ReactiveCommand<Unit, IFilter<T>> Update { get; }
    }
}
