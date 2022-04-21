using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using NetTopologySuite.IO;
using System.Collections.Generic;

namespace InteractiveSample.Layers
{
    public class UserLayer : BaseLayer
    {
        private readonly WritableLayer _source;

        public UserLayer() : base()
        {
            _source = new WritableLayer();
        }

        public void AddFeature(IFeature feature)
        {
            _source.Add(feature);
        }

        public void AddFeature(string name, string wkt, IStyle? style = null)
        {
            var g = new WKTReader().Read(wkt);

            var feature = g.ToFeature();

            feature["Name"] = name;

            if (style != null)
            {
                feature.Styles = new[] { style };
            }

            _source.Add(feature);
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution) => _source.GetFeatures(box, resolution);

        public override void RefreshData(FetchInfo fetchInfo) => _source.RefreshData(fetchInfo);
    }
}
