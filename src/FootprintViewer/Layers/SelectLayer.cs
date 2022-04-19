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

        //public override BoundingBox Envelope => _source.Envelope;

        public SelectLayer(ILayer source, IFeature feature)
        {
            _source = source;
            _feature = feature;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
            
            IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            if (box.Intersects(_feature.Extent/* Geometry?.BoundingBox*/) == true)
            {
                yield return _feature;
            }

            //return new Feature[] { };

            //return _source.GetFeaturesInView(box, resolution);
        }
        
        public override void RefreshData(FetchInfo fetchInfo)
        //public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
