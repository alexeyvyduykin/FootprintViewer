using FootprintViewer.Data;
using ReactiveUI;
using System.Reactive;

namespace FootprintViewer.ViewModels
{
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
