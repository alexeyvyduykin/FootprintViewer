using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;

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

            LayerChanged = ReactiveCommand.Create<LayerSource, LayerSource>(s => s);

            this.WhenAnyValue(x => x.SelectedLayer).InvokeCommand(LayerChanged);
        }

        public ReactiveCommand<LayerSource, LayerSource> LayerChanged { get; }

        [Reactive]
        public ObservableCollection<LayerSource> Layers { get; set; }

        [Reactive]
        public LayerSource SelectedLayer { get; set; }
    }
}
