using DynamicData;
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
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.ToolBar;

public class LayerContainerViewModel : ViewModelBase
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
           .Transform(s => new LayerItemViewModel(s, _layerStyleManager.GetStyles(s.Name)))
           .Bind(out _layerItems)
           .Subscribe();

        _layerItems
            .ToObservableChangeSet()
            .WhenPropertyChanged(p => p.IsVisible)
            .Subscribe(_ => LayerItemChangedImpl());

        this.WhenAnyValue(s => s.IsAllVisible)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(IsAllVisibleChanged);

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

    private void LayerItemChangedImpl()
    {
        var allTrue = LayerItems.All(s => s.IsVisible == true);
        var allFalse = LayerItems.All(s => s.IsVisible == false);
        var anyFalse = LayerItems.Any(s => s.IsVisible == false);

        if (allTrue == true)
        {
            IsAllVisible = true;
        }
        else if (allFalse == true)
        {
            IsAllVisible = false;
        }
        else if (anyFalse == true)
        {
            IsAllVisible = null;
        }
    }

    private void IsAllVisibleChanged(bool? value)
    {
        if (value is bool visible)
        {
            foreach (var item in LayerItems)
            {
                item.IsVisible = visible;
            }
        }
    }

    [Reactive]
    public bool? IsAllVisible { get; set; }

    public IReadOnlyList<LayerItemViewModel> LayerItems => _layerItems;
}
