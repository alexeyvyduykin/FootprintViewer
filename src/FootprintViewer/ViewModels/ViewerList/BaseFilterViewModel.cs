using FootprintViewer.Data;
using ReactiveUI;
using System;

namespace FootprintViewer.ViewModels
{
    public abstract class BaseFilterViewModel<T> : ReactiveObject, IFilter<T> where T : IViewerItem
    {
        public abstract IObservable<Func<T, bool>> FilterObservable { get; }

        public abstract string[]? Names { get; }

        public abstract bool Filtering(T value);
    }
}
