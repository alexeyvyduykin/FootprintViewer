using Mapsui;
using Mapsui.Geometries;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using System.Collections.Generic;

namespace InteractivitySample.Layers
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
            var g = GeometryFromWKT.Parse(wkt);
            var feature = new Feature { Geometry = g };
            feature["Name"] = name;

            if (style != null)
            {
                feature.Styles = new[] { style };
            }

            _source.Add(feature);
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution) => _source.GetFeaturesInView(box, resolution);

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType) => _source.RefreshData(extent, resolution, changeType);
    }
}
