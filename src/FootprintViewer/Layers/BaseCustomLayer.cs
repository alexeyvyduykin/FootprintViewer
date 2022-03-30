using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class BaseCustomLayer : BaseLayer
    {
        private readonly ILayer _source;

        public BaseCustomLayer(ILayer source)
        {
            _source = source;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
        }

        protected ILayer Source => _source;

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            foreach (var feature in _source.GetFeaturesInView(box, resolution))
            {
                yield return feature;
            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
