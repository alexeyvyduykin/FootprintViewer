using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class WorldMapSelector : ReactiveObject
    {
        private readonly MapProvider _mapProvider;
        private readonly ObservableAsPropertyHelper<List<MapResource>> _worldMaps;

        public WorldMapSelector(IReadonlyDependencyResolver dependencyResolver)
        {
            _mapProvider = dependencyResolver.GetExistingService<MapProvider>();

            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            Loading = ReactiveCommand.CreateFromTask(LoadingImpl);

            _worldMaps = Loading.ToProperty(this, x => x.WorldMaps);

            this.WhenAnyValue(x => x.SelectedWorldMap!).InvokeCommand(WorldMapChanged);
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ReactiveCommand<Unit, List<MapResource>> Loading { get; }

        private async Task<List<MapResource>> LoadingImpl()
        {
            return await _mapProvider.GetValuesAsync();
        }

        private List<MapResource> WorldMaps => _worldMaps.Value;

        [Reactive]
        public MapResource? SelectedWorldMap { get; set; }
    }
}
