using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels;

public class NameFilter<T> : ViewModelBase, IFilter<T> where T : IViewerItem
{
    public NameFilter() : base()
    {

    }

    public IObservable<Func<T, bool>> FilterObservable =>
        this.WhenAnyValue(s => s.FilterNames)
        .Throttle(TimeSpan.FromSeconds(1))
        .Select(_ => this)
        .Select(CreatePredicate);

    private static Func<T, bool> CreatePredicate(NameFilter<T> filter)
    {
        return s => filter.Filtering(s);
    }

    [Reactive]
    public string[]? FilterNames { get; set; }

    private bool Filtering(T value)
    {
        return (FilterNames == null) || FilterNames.Contains(value.Name);
    }
}
