﻿using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Styles;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.ToolBar;

public sealed class LayerContainerViewModel : ViewModelBase
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;
    private readonly LayerStyleManager _layerStyleManager;
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public LayerContainerViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;
        _layerStyleManager = _dependencyResolver.GetExistingService<LayerStyleManager>();

        _layers
           .Connect()
           .ObserveOn(RxApp.MainThreadScheduler)
           .Transform(s => new LayerItemViewModel(s, _layerStyleManager))
           .Bind(out _layerItems)
           .Subscribe();

        _layerItems
            .ToObservableChangeSet()
            .WhenPropertyChanged(p => p.IsVisible)
            .Select(_ => LayerItems.AllPropertyCheck(p => p.IsVisible))
            .Subscribe(s => IsAllVisible = s);

        this.WhenAnyValue(s => s.IsAllVisible)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Where(s => s != null)
            .Select(s => (bool)s!)
            .Subscribe(value => LayerItems.SetValue(s => s.IsVisible = value));

        Observable.StartAsync(() => Task.Run(UpdateLayers), RxApp.MainThreadScheduler).Subscribe();
    }

    private void UpdateLayers()
    {
        var map = (Map)_dependencyResolver.GetExistingService<IMap>();

        _layers.Edit(innerList =>
        {
            innerList.Clear();
            innerList.AddRange(map.Layers);
        });
    }

    [Reactive]
    public bool? IsAllVisible { get; set; }

    public IReadOnlyList<LayerItemViewModel> LayerItems => _layerItems;
}
