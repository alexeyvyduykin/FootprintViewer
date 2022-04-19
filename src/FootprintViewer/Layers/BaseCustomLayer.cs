using Mapsui;
using Mapsui.Fetcher;
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

        //public override BoundingBox Envelope => _source.Envelope;

        protected ILayer Source => _source;

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            return _source.GetFeatures(box, resolution);

            //foreach (var feature in _source.GetFeaturesInView(box, resolution))
            //{
            //    yield return feature;
            //}
        }

        //public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        public override void RefreshData(FetchInfo fetchInfo)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
