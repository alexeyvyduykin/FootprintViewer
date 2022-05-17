using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class WorldMapSelector : ReactiveObject
    {
        public WorldMapSelector(IReadonlyDependencyResolver dependencyResolver)
        {
            var provider = dependencyResolver.GetExistingService<MapProvider>();

            WorldMaps = new ObservableCollection<MapResource>();

            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            var resources = provider.GetMapResources();

            WorldMaps = new ObservableCollection<MapResource>(resources);

            SelectedWorldMap = WorldMaps.FirstOrDefault()!;

            this.WhenAnyValue(x => x.SelectedWorldMap).InvokeCommand(WorldMapChanged);
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ObservableCollection<MapResource> WorldMaps { get; }

        [Reactive]
        public MapResource SelectedWorldMap { get; set; }
    }
}
