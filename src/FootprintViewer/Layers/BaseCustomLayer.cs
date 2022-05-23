using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class BaseCustomLayer<T> : BaseLayer where T : ILayer
    {
        private readonly T _source;

        public BaseCustomLayer(T source)
        {
            _source = source;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
        }

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
