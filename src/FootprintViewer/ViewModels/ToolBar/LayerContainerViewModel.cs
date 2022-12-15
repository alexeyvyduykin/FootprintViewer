using DynamicData;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels.ToolBar;

public class LayerContainerViewModel : ViewModelBase
{
    private readonly IReadonlyDependencyResolver _dependencyResolver;
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public LayerContainerViewModel(IReadonlyDependencyResolver dependencyResolver)
    {
        _dependencyResolver = dependencyResolver;

        _layers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Transform(s => new LayerItemViewModel(s))
            .Bind(out _layerItems)
            .Subscribe();

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

    public IReadOnlyList<LayerItemViewModel> LayerItems => _layerItems;
}
