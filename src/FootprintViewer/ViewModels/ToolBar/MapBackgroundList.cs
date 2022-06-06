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
        private readonly ObservableAsPropertyHelper<List<MapResource>> _mapBackgrounds;

        public MapBackgroundList(IReadonlyDependencyResolver dependencyResolver)
        {
            _mapProvider = dependencyResolver.GetExistingService<IProvider<MapResource>>();

            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            Loading = ReactiveCommand.CreateFromTask(s => _mapProvider.GetValuesAsync(null));

            _mapBackgrounds = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.MapBackgrounds);

            this.WhenAnyValue(x => x.SelectedMapBackground!).InvokeCommand(WorldMapChanged);
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ReactiveCommand<Unit, List<MapResource>> Loading { get; }

        public List<MapResource> MapBackgrounds => _mapBackgrounds.Value;

        [Reactive]
        public MapResource? SelectedMapBackground { get; set; }
    }
}
