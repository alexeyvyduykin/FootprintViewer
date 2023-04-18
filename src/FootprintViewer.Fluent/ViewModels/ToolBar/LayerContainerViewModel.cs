using DynamicData;
using DynamicData.Binding;
using FootprintViewer.Styles;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public sealed class LayerContainerViewModel : ViewModelBase
{
    private readonly LayerStyleManager _layerStyleManager;
    private readonly SourceList<ILayer> _layers = new();
    private readonly ReadOnlyObservableCollection<LayerItemViewModel> _layerItems;

    public LayerContainerViewModel()
    {
        _layerStyleManager = Services.LayerStyleManager;

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

        Observable.StartAsync(UpdateLayersAsync, RxApp.MainThreadScheduler);
    }

    private async Task UpdateLayersAsync() => await Observable.Start(() => UpdateLayers(), RxApp.TaskpoolScheduler);

    private void UpdateLayers()
    {
        var map = Services.Map;

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
