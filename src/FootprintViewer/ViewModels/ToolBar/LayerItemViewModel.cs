using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.ToolBar;

public class LayerItemViewModel : ViewModelBase
{
    public LayerItemViewModel(ILayer layer)
    {
        Name = layer.Name;

        IsVisible = layer.Enabled;

        this.WhenAnyValue(s => s.IsVisible)
            .Subscribe(s => layer.Enabled = s);
    }

    public string Name { get; set; }

    [Reactive]
    public bool IsVisible { get; set; }
}
