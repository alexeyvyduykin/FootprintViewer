using DynamicData;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers;
using FootprintViewer.Layers.Providers;
using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Layers;
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
    private readonly IDataManager _dataManager;
    private readonly SourceList<GroundTarget> _groundTargets = new();
    private readonly ReadOnlyObservableCollection<GroundTargetViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly ObservableAsPropertyHelper<bool> _isEnable;
    private readonly LayerManager? _targetManager;

    public GroundTargetTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        var map = dependencyResolver.GetExistingService<IMap>();
        var layer = map.GetLayer<Layer>(LayerType.GroundTarget);
        _targetManager = layer?.BuildManager(() => ((GroundTargetProvider)layer.DataSource!).ActiveFeatures);

        Title = "Просмотр наземных целей";

        Filter = new GroundTargetNameFilterViewModel(dependencyResolver);

        _groundTargets
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new GroundTargetViewModel(s))
            .Filter(Filter.FilterObservable)
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _dataManager.DataChanged
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.SelectedItem)
            .InvokeCommand(ReactiveCommand.Create<GroundTargetViewModel?>(SelectImpl));

        Enter = ReactiveCommand.Create<GroundTargetViewModel>(EnterImpl);

        Leave = ReactiveCommand.Create(LeaveImpl);

        _isLoading = Update.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s => Filter.IsActive = s);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .ToSignal()
            .InvokeCommand(Update);

        _isEnable = Filter.EnableFilterObservable
            .ToProperty(this, x => x.IsEnable);
    }

    protected GroundTargetNameFilterViewModel Filter { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public ReactiveCommand<GroundTargetViewModel, Unit> Enter { get; }

    public ReactiveCommand<Unit, Unit> Leave { get; }

    private async Task UpdateImpl()
    {
        var res = await _dataManager.GetDataAsync<GroundTarget>(DbKeys.GroundTargets.ToString());

        _groundTargets.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
    }

    private void EnterImpl(GroundTargetViewModel groundTarget)
    {
        var name = groundTarget.Name;

        if (string.IsNullOrEmpty(name) == false)
        {
            _targetManager?.ShowHighlight(name);
        }
    }

    private void LeaveImpl()
    {
        _targetManager?.HideHighlight();
    }

    private void SelectImpl(GroundTargetViewModel? groundTarget)
    {
        if (groundTarget != null)
        {
            var name = groundTarget.Name;

            if (string.IsNullOrEmpty(name) == false)
            {
                _targetManager?.SelectFeature(name);
            }
        }
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
