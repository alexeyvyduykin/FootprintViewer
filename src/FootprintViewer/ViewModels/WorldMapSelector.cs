using FootprintViewer.Data;
using FootprintViewer.Models;
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
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class WorldMapSelector : ReactiveObject
    {
        public WorldMapSelector(IReadonlyDependencyResolver dependencyResolver)
        {
            var userDataSource = dependencyResolver.GetService<IUserDataSource>();

            var layers = userDataSource?.WorldMapSources;

            Layers = new ObservableCollection<LayerSource>(layers);

            SelectedLayer = layers.FirstOrDefault();

            SelectedLayerObserver = this.WhenAnyValue(x => x.SelectedLayer);

            SelectedLayerObserver.Subscribe(layerSource =>
            {
                SelectLayer?.Invoke(layerSource);
                    
                IsOpen = false;                
            });

            ClickCommand = ReactiveCommand.Create(Click);
        }

        public void Click()
        {
            IsOpen = !IsOpen;
        }

        public IObservable<LayerSource> SelectedLayerObserver { get; private set; }


        public Action<LayerSource>? SelectLayer;

        public ReactiveCommand<Unit, Unit> ClickCommand { get; }

        [Reactive]
        public bool IsOpen { get; set; }

        [Reactive]
        public ObservableCollection<LayerSource> Layers { get; set; }

        [Reactive]
        public LayerSource SelectedLayer { get; set; }
    }
}
