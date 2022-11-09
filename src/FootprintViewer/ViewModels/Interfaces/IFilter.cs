using System;

namespace FootprintViewer.ViewModels;

public interface IFilter<T>
{
    IObservable<Func<T, bool>> FilterObservable { get; }
}
