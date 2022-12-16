using FootprintViewer.Styles;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.ToolBar;

public class LayerItemViewModel : ViewModelBase
{
    public LayerItemViewModel(ILayer layer, LayerStyleManager layerStyleManager)
    {
        Name = layer.Name;

        IsVisible = layer.Enabled;

        this.WhenAnyValue(s => s.IsVisible)
            .Subscribe(s => layer.Enabled = s);

        var styles = layerStyleManager.GetStyles(layer.Name);
        var selectedStyle = layerStyleManager.GetStyle(layer.Name);

        Styles = new List<string>(styles?.Select(s => s.Name) ?? Array.Empty<string>());

        SelectedStyle = selectedStyle?.Name ?? Styles.FirstOrDefault();

        this.WhenAnyValue(s => s.SelectedStyle)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(s =>
            {
                var res = styles?.Where(style => Equals(style.Name, s)).FirstOrDefault();

                if (res != null)
                {
                    layerStyleManager.Select(layer, res);

                    layer.DataHasChanged();
                }
            });
    }

    public string Name { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public string? SelectedStyle { get; set; }

    [Reactive]
    public List<string> Styles { get; set; }
}
