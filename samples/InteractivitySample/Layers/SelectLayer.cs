using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System.Collections.Generic;
using System.Linq;

namespace InteractivitySample.Layers
{
    public class SelectLayer : BaseLayer
    {
        private readonly ILayer _source;

        private IFeature _feature;

        public override BoundingBox Envelope => _source.Envelope;

        public SelectLayer(ILayer source, IFeature feature)
        {
            _source = source;
            _feature = feature;
            _source.DataChanged += (sender, args) => OnDataChanged(args);                      
        }

        //public SelectLayer(ILayer source)
        //{
        //    _source = source;
        //    _source.DataChanged += (sender, args) => OnDataChanged(args);         
        //}

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (box.Intersects(_feature.Geometry?.BoundingBox) == true)
            {
               yield return _feature;
            }

            //return new Feature[] { };

            //return _source.GetFeaturesInView(box, resolution);
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
