using FootprintViewer.Styles;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public class LayerItemViewModel : ViewModelBase
{
    public LayerItemViewModel(ILayer layer, LayerStyleManager layerStyleManager)
    {
        Name = layer.Name;

        IsVisible = layer.Enabled;

        Styles = layerStyleManager.GetStyles(layer.Name).ToList();

        SelectedStyle = layerStyleManager.GetStyle(layer.Name);

        this.WhenAnyValue(s => s.IsVisible)
            .Subscribe(s => layer.Enabled = s);

        this.WhenAnyValue(s => s.SelectedStyle)
            .ObserveOn(RxApp.MainThreadScheduler)
            .WhereNotNull()
            .Subscribe(s =>
            {
                layerStyleManager.Select(layer, s);
                layer.DataHasChanged();
            });
    }

    public string Name { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public LayerStyleViewModel? SelectedStyle { get; set; }

    public List<LayerStyleViewModel> Styles { get; set; }
}
