using DynamicData;
using DynamicData.Binding;
using FootprintViewer.UI.Services2;
using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public sealed class LayerContainerViewModel : ViewModelBase
{
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public LayerContainerViewModel()
    {
        var mapService = Services.Locator.GetRequiredService<IMapService>();

        _layers
           .Connect()
           .ObserveOn(RxApp.MainThreadScheduler)
           .Transform(s => new LayerItemViewModel(s, mapService.LayerStyle))
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

        Observable.StartAsync(() => UpdateLayersAsync(mapService.Map), RxApp.MainThreadScheduler);
    }

    private async Task UpdateLayersAsync(IMap? map) => await Observable.Start(() => UpdateLayers(map), RxApp.TaskpoolScheduler);

    private void UpdateLayers(IMap? map)
    {
        if (map is { })
        {
            _layers.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(map.Layers);
            });
        }
    }

    [Reactive]
    public bool? IsAllVisible { get; set; }

    public IReadOnlyList<LayerItemViewModel> LayerItems => _layerItems;
}