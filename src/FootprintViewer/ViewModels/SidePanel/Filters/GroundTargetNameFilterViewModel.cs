using FootprintViewer.Layers.Providers;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.SidePanel.Filters;

public class GroundTargetNameFilterViewModel : ViewModelBase, IFilter<GroundTargetViewModel>
{
    private readonly IObservable<Func<GroundTargetViewModel, bool>> _filterObservable;
    private readonly IObservable<bool> _enableFilterObservable;
    private readonly ObservableAsPropertyHelper<string[]?> _names;

    public GroundTargetNameFilterViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        var provider = dependencyResolver.GetExistingService<GroundTargetProvider>();

        _names = provider.ActiveFeaturesChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.Names);

        _filterObservable = this.WhenAnyValue(s => s.Names, s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s.Item2 == true)
            .Where(s => s.Item1 != null)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this)
            .Select(CreatePredicate);

        _enableFilterObservable = this.WhenAnyValue(s => s.Names, s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(s => s.Item1 != null);
    }

    public IObservable<Func<GroundTargetViewModel, bool>> FilterObservable => _filterObservable;

    public IObservable<bool> EnableFilterObservable => _enableFilterObservable;

    private static Func<GroundTargetViewModel, bool> CreatePredicate(GroundTargetNameFilterViewModel filter)
    {
        return s => filter.Filtering(s);
    }

    private string[]? Names => _names.Value;

    [Reactive]
    public bool IsActive { get; set; }

    private bool Filtering(GroundTargetViewModel value)
    {
        return Names?.Contains(value.Name) ?? false;
    }
}
