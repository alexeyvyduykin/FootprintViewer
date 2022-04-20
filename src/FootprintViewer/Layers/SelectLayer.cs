using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class SelectLayer : BaseLayer
    {
        private readonly ILayer _source;

        private readonly IFeature _feature;

        public SelectLayer(ILayer source, IFeature feature)
        {
            _source = source;
            _feature = feature;
            _source.DataChanged += (sender, args) => OnDataChanged(args);

            IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            if (box.Intersects(_feature.Extent) == true)
            {
                yield return _feature;
            }
        }

        public override void RefreshData(FetchInfo fetchInfo)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
