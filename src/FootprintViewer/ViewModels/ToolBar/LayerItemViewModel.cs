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
    public LayerItemViewModel(ILayer layer, LayerStyleViewModel[]? styles)
    {
        Name = layer.Name;

        IsVisible = layer.Enabled;

        this.WhenAnyValue(s => s.IsVisible)
            .Subscribe(s => layer.Enabled = s);

        Styles = new List<string>(styles?.Select(s => s.Name) ?? Array.Empty<string>());

        SelectedStyle = Styles.FirstOrDefault();
    }

    public string Name { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }

    [Reactive]
    public string? SelectedStyle { get; set; }

    [Reactive]
    public List<string> Styles { get; set; }
}
