using Mapsui;
using Mapsui.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class MapLayerItem : ReactiveObject
    {
        public MapLayerItem(ILayer layer)
        {
            Name = layer.Name;

            IsVisible = layer.Enabled;

            this.WhenAnyValue(s => s.IsVisible).Subscribe(s => layer.Enabled = s);
        }

        public string Name { get; set; }

        [Reactive]
        public bool IsVisible { get; set; }
    }

    public class MapLayerList : ReactiveObject, IActivatableViewModel
    {
        private readonly Map _map;

        public ViewModelActivator Activator { get; }

        public MapLayerList(IReadonlyDependencyResolver dependencyResolver)
        {
            Activator = new ViewModelActivator();

            _map = (Map)dependencyResolver.GetExistingService<IMap>();

            Layers = new List<MapLayerItem>();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Layers = _map.Layers.Select(s => new MapLayerItem(s)).ToList();
            });
        }

        public List<MapLayerItem> Layers { get; private set; }
    }
}
