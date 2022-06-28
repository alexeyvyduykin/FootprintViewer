using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class SceneSearch : SidePanelTab
    {
        private readonly IProvider<(string, NetTopologySuite.Geometries.Geometry)> _footprintPreviewGeometryProvider;
        private readonly ObservableAsPropertyHelper<IDictionary<string, NetTopologySuite.Geometries.Geometry>> _geometries;

        public SceneSearch(IReadonlyDependencyResolver dependencyResolver)
        {
            var footprintPreviewProvider = dependencyResolver.GetExistingService<IProvider<Data.FootprintPreview>>();
            _footprintPreviewGeometryProvider = dependencyResolver.GetExistingService<IProvider<(string, NetTopologySuite.Geometries.Geometry)>>();

            //ViewerList = ViewerListBuilder.CreateViewerList(footprintPreviewProvider, s => new FootprintPreview(s.Name), s => new Data.FootprintPreview());

            Filter = new SceneSearchFilter(dependencyResolver);

            Title = "Поиск сцены";

            LoadFootprintPreviewGeometry = ReactiveCommand.CreateFromTask(_ => _footprintPreviewGeometryProvider.GetNativeValuesAsync(null));

            // TODO: duplicates           
            _geometries = LoadFootprintPreviewGeometry.Select(s => s.ToDictionary(s => s.Item1, s => s.Item2)).ToProperty(this, x => x.Geometries);

            // First loading

            this.WhenAnyValue(s => s.IsActive)
                .Take(2)
                .Where(active => active == true)
                .Select(_ => Unit.Default)
                .InvokeCommand(LoadFootprintPreviewGeometry);

            this.WhenAnyValue(s => s.IsActive)
                .Take(2)
                .Where(active => active == true)
                .Select(_ => Filter)
                .InvokeCommand(ViewerList.Loading);

            // Filter

            Filter.Update.InvokeCommand(ViewerList.Loading);
        }

        public void SetAOI(NetTopologySuite.Geometries.Geometry aoi) => ((SceneSearchFilter)Filter).AOI = aoi;

        public void ResetAOI() => ((SceneSearchFilter)Filter).AOI = null;

        private ReactiveCommand<Unit, List<(string, NetTopologySuite.Geometries.Geometry)>> LoadFootprintPreviewGeometry { get; }

        [Reactive]
        public IViewerList<FootprintPreview> ViewerList { get; private set; }

        [Reactive]
        public IFilter<FootprintPreview> Filter { get; private set; }

        public IDictionary<string, NetTopologySuite.Geometries.Geometry> Geometries => _geometries.Value;
    }
}
