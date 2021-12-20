using Mapsui.Geometries;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InteractivitySample.ViewModels
{
    public class CustomProvider : MemoryProvider
    {
        private IFeature _feature;

        public CustomProvider(string name, string wkt)
        {
            var g = GeometryFromWKT.Parse(wkt);
            _feature = new Feature { Geometry = g };
            _feature["Name"] = name;

            ReplaceFeatures(new[] { _feature });
        }
    }
}
