using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
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

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            return _source.GetFeatures(box, resolution);
        }

        public override void RefreshData(FetchInfo fetchInfo)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
