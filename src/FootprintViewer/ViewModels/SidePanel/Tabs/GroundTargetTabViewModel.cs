using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.ViewModels.SidePanel.Items;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class GroundTargetTabViewModel : SidePanelTabViewModel
{
    private readonly Data.DataManager.IDataManager _dataManager;
    private readonly ITargetLayerSource _source;
    private readonly SourceList<GroundTargetViewModel> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _isEnable;
    private readonly ObservableAsPropertyHelper<string[]?> _names;

    public GroundTargetTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<Data.DataManager.IDataManager>();
        _source = dependencyResolver.GetExistingService<ITargetLayerSource>();

        Title = "Просмотр наземных целей";

        NameFilter = new NameFilter<GroundTargetViewModel>(Array.Empty<string>());

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Filter(NameFilter.FilterObservable)
            .Bind(out _items)
            .Subscribe();

        Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

        Delay = ReactiveCommand.CreateFromTask(() => Task.Delay(TimeSpan.FromSeconds(1.0)));

        Enter = ReactiveCommand.Create<GroundTargetViewModel, GroundTargetViewModel>(s => s);

        Leave = ReactiveCommand.Create(() => { });

        _isLoading = Delay.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        _names = _source.Refresh
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.Names);

        _source.Refresh
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(_ => Unit.Default)
            .InvokeCommand(Delay);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(active => active == true)
            .Take(1)
            .Select(_ => Unit.Default)
            .InvokeCommand(Loading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(active => active == true)
            .Select(_ => Unit.Default)
            .InvokeCommand(Delay);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(active => active == true)
            .Subscribe(_ => NameFilter.FilterNames = Names);

        this.WhenAnyValue(s => s.Names)
            .Where(_ => IsActive == true)
            .Subscribe(names => NameFilter.FilterNames = names);

        _isEnable = this.WhenAnyValue(s => s.NameFilter.FilterNames)
            .Select(s => s != null)
            .ToProperty(this, x => x.IsEnable);
    }

    protected NameFilter<GroundTargetViewModel> NameFilter { get; }

    public IObservable<GroundTargetViewModel?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedItem);

    public ReactiveCommand<Unit, Unit> Loading { get; }

    private ReactiveCommand<Unit, Unit> Delay { get; }

    public ReactiveCommand<GroundTargetViewModel, GroundTargetViewModel> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    private string[]? Names => _names.Value;

    private async Task LoadingImpl()
    {
        var res = await _dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

        var list = res.Select(s => new GroundTargetViewModel(s)).ToList();

        _groundTargets.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(list);
        });
    }

    public async Task<List<GroundTargetViewModel>> GetGroundTargetViewModelsAsync(string name)
    {
        var res = await _dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

        var list = res
            .Where(s => string.Equals(s.Name, name))
            .Select(s => new GroundTargetViewModel(s))
            .ToList();

        return list;
    }

    public bool IsLoading => _isLoading.Value;

    public bool IsEnable => _isEnable.Value;

    public ReadOnlyObservableCollection<GroundTargetViewModel> Items => _items;

    [Reactive]
    public GroundTargetViewModel? SelectedItem { get; set; }
}
