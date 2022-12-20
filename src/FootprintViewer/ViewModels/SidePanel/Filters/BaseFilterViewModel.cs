using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public abstract class BaseFilterViewModel<T> : ViewModelBase, IFilter<T> where T : ViewModelBase
{
    private readonly IObservable<Func<T, bool>> _filterObservable;
    private readonly ObservableAsPropertyHelper<bool> _isDirty;
    private readonly SourceList<IObservable<BaseFilterViewModel<T>>> _mergeObservables = new();
    private readonly SourceList<IObservable<BaseFilterViewModel<T>>> _dirtyMergeObservables = new();

    public BaseFilterViewModel()
    {
        var merged = _mergeObservables
            .Connect()
            .MergeMany(s => s);

        _filterObservable = merged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(CreatePredicate);

        var dirtyMerged = _dirtyMergeObservables
            .Connect()
            .MergeMany(s => s);

        var obs1 = dirtyMerged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => IsNotDefault(s));

        Reset = ReactiveCommand.Create(ResetImpl, outputScheduler: RxApp.MainThreadScheduler);

        var obs2 = Reset.Select(_ => false);

        _isDirty = Observable.Merge(obs1, obs2)
            .ToProperty(this, x => x.IsDirty);
    }

    protected abstract bool Filtering(T viewModel);

    protected abstract void ResetImpl();

    protected abstract bool IsDefaultImpl();

    private static bool IsNotDefault(BaseFilterViewModel<T> filter)
    {
        return !filter.IsDefaultImpl();
    }

    protected void SetMergeObservables(IList<IObservable<object>> observables)
    {
        _mergeObservables.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(observables
                .Where(s => s is IObservable<BaseFilterViewModel<T>>)
                .Cast<IObservable<BaseFilterViewModel<T>>>()
                .ToList());
        });
    }

    protected void SetDirtyMergeObservables(IList<IObservable<object>> observables)
    {
        _dirtyMergeObservables.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(observables
                .Where(s => s is IObservable<BaseFilterViewModel<T>>)
                .Cast<IObservable<BaseFilterViewModel<T>>>()
                .ToList());
        });
    }

    private static Func<T, bool> CreatePredicate(BaseFilterViewModel<T> filter)
    {
        return s => filter.Filtering(s);
    }

    public IObservable<Func<T, bool>> FilterObservable => _filterObservable;

    public ReactiveCommand<Unit, Unit> Reset { get; }

    public bool IsDirty => _isDirty.Value;
}
