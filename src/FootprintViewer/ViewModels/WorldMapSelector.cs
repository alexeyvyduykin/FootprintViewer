using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class WorldMapSelector : ReactiveObject
    {
        private readonly IProvider<MapResource> _mapProvider;
        private readonly ObservableAsPropertyHelper<List<MapResource>> _worldMaps;

        public WorldMapSelector(IReadonlyDependencyResolver dependencyResolver)
        {
            _mapProvider = dependencyResolver.GetExistingService<IProvider<MapResource>>();

            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            Loading = ReactiveCommand.Create<List<MapResource>, List<MapResource>>(s => s);

            _mapProvider.Loading.InvokeCommand(Loading);

            _worldMaps = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.WorldMaps);

            this.WhenAnyValue(x => x.SelectedWorldMap!).InvokeCommand(WorldMapChanged);
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ReactiveCommand<List<MapResource>, List<MapResource>> Loading { get; }

        public List<MapResource> WorldMaps => _worldMaps.Value;

        [Reactive]
        public MapResource? SelectedWorldMap { get; set; }
    }
}
