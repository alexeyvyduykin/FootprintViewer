using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class MapBackgroundList : ReactiveObject
    {
        private readonly IProvider<MapResource> _mapProvider;
        private readonly ObservableAsPropertyHelper<List<MapResource>> _worldMaps;

        public MapBackgroundList(IReadonlyDependencyResolver dependencyResolver)
        {
            _mapProvider = dependencyResolver.GetExistingService<IProvider<MapResource>>();

            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            Loading = ReactiveCommand.CreateFromTask(s => _mapProvider.GetValuesAsync(null));

            _worldMaps = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.WorldMaps);

            this.WhenAnyValue(x => x.SelectedWorldMap!).InvokeCommand(WorldMapChanged);
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ReactiveCommand<Unit, List<MapResource>> Loading { get; }

        public List<MapResource> WorldMaps => _worldMaps.Value;

        [Reactive]
        public MapResource? SelectedWorldMap { get; set; }
    }
}
