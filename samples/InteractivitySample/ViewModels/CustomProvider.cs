using Mapsui.Geometries;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractivitySample.ViewModels
{
    public class CustomProvider : MemoryProvider
    {
        private IList<IFeature> _features = new List<IFeature>();

        public CustomProvider()
        {

        }

        public void AddFeature(IFeature feature)
        {
            _features.Add(feature);

            ReplaceFeatures(_features);
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

            _features.Add(feature);

            ReplaceFeatures(_features);
        }
    }
}
