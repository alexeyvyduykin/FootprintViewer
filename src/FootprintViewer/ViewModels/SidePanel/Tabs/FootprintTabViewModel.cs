﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Data;
using FootprintViewer.Data.DataManager;
using FootprintViewer.Layers.Providers;
using FootprintViewer.Styles;
using FootprintViewer.ViewModels.SidePanel.Filters;
using FootprintViewer.ViewModels.SidePanel.Items;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.SidePanel.Tabs;

public class FootprintTabViewModel : SidePanelTabViewModel
{
    private readonly IDataManager _dataManager;
    private readonly IMapNavigator _mapNavigator;
    private readonly SourceList<Footprint> _footprints = new();
    private readonly ReadOnlyObservableCollection<FootprintViewModel> _items;
    private readonly ObservableAsPropertyHelper<bool> _isLoading;
    private readonly FeatureManager _featureManager;
    private readonly ILayer? _layer;
    private readonly FootprintProvider _provider;

    public FootprintTabViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dataManager = dependencyResolver.GetExistingService<IDataManager>();
        _mapNavigator = dependencyResolver.GetExistingService<IMapNavigator>();
        var map = dependencyResolver.GetExistingService<IMap>();
        _layer = map.GetLayer(LayerType.Footprint);
        _provider = dependencyResolver.GetExistingService<FootprintProvider>();
        _featureManager = dependencyResolver.GetExistingService<FeatureManager>();

        Filter = new FootprintTabFilterViewModel(dependencyResolver);

        Title = "Просмотр рабочей программы";

        _footprints
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new FootprintViewModel(s))
            .Filter(Filter.FilterObservable)
            .Sort(SortExpressionComparer<FootprintViewModel>.Ascending(s => s.Begin))
            .Bind(out _items)
            .Subscribe();

        Update = ReactiveCommand.CreateFromTask(UpdateImpl);

        _isLoading = Update.IsExecuting
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .ToProperty(this, x => x.IsLoading);

        _dataManager.DataChanged
            .Where(s => s.Contains(DbKeys.Footprints.ToString()))
            .ToSignal()
            .InvokeCommand(Update);

        this.WhenAnyValue(s => s.IsActive)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereTrue()
            .Take(1)
            .ToSignal()
            .InvokeCommand(Update);

        TargetToMap = ReactiveCommand.CreateFromTask<FootprintViewModel>(
            s => Task.Run(() =>
            {
                _mapNavigator.FlyToFootprint(s.Center);

                _featureManager
                    .OnLayer(_layer)
                    .Select(_provider.Find(s.Name, "Name"));
            }), outputScheduler: RxApp.MainThreadScheduler);
    }

    public ReactiveCommand<FootprintViewModel, Unit> TargetToMap { get; }

    public ReactiveCommand<Unit, Unit> Update { get; }

    public bool IsLoading => _isLoading.Value;

    private async Task UpdateImpl()
    {
        await Task.Delay(TimeSpan.FromSeconds(1));

        var res = await _dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

        _footprints.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(res);
        });
    }

    public FootprintViewModel? GetFootprintViewModel(string name)
    {
        return Items.Where(s => s.Name.Equals(name)).FirstOrDefault();
    }

    public async Task<List<FootprintViewModel>> GetFootprintViewModelsAsync(string name)
    {
        var res = await _dataManager.GetDataAsync<Footprint>(DbKeys.Footprints.ToString());

        var list = res
            .Where(s => string.Equals(s.Name, name))
            .Select(s => new FootprintViewModel(s))
            .ToList();

        return list;
    }

    public IFilter<FootprintViewModel> Filter { get; }

    public ReadOnlyObservableCollection<FootprintViewModel> Items => _items;
}
