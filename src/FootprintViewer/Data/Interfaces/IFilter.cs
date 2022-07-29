using FootprintViewer.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;

namespace FootprintViewer.Data
{
    public interface IFilter<T>
    {
        bool Filtering(T value);

        string[]? Names { get; }
    
        IObservable<Func<T, bool>> FilterObservable { get; }
    }
}
