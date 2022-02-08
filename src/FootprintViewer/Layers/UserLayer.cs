using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class UserLayer : BaseLayer
    {
        //private readonly WritableLayer _source;
        private readonly CustomProvider _source;
      
        public UserLayer(CustomProvider provider) : base()
        {
            _source = provider;
            //_source = new WritableLayer();

            //provider.OnAdd += Provider_OnAdd;
        }

        private void Provider_OnAdd(object? sender, System.EventArgs e)
        {
            if (sender is IFeature feature)
            {
                _source.Add(feature);
            }
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution) => _source.GetFeaturesInView(box, resolution);

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType) => _source.RefreshData(extent, resolution, changeType);
    }
}
