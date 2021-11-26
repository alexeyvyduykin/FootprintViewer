using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.Generic;
using System.Linq;

namespace SatelliteGeometrySample
{
    public class FootprintLayer : MemoryLayer
    {
        private IStyle _previewStyle;
        private IStyle _style;

        public FootprintLayer(IProvider provider)
        {
            _previewStyle = CreateFootprintPreviewThemeStyle();
            _style = CreateFootprintThemeStyle();

            DataSource = provider;
           IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (resolution >= 10000) // preview
            {
                Style = _previewStyle;
                foreach (var feature in base.GetFeaturesInView(box, resolution))
                {
                    var center = feature.Geometry.BoundingBox.Centroid;

                    yield return new Feature()
                    {                    
                        ["Name"] = feature["Name"],
                        ["State"] = feature["State"],
                        Geometry = center,
                    };
                }
            }
            else
            {
                Style = _style;

                foreach (var feature in base.GetFeaturesInView(box, resolution))
                {
                    yield return feature;
                }
            }
        }

        private static IStyle CreateFootprintThemeStyle()
        {
            var stl = new ThemeStyle(f =>
            {
                if (f.Geometry is Point)
                {
                    return null;
                }

                var style = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = 10000,
                };

                switch (f["State"].ToString())
                {
                    case "Unselect":
                        style.Fill = new Brush(Color.Opacity(Color.Green, 0.25f));
                        style.Line = new Pen(Color.Green, 1.0);
                        style.Outline = new Pen(Color.Green, 1.0);
                        break;
                    case "Select":
                        style.Fill = new Brush(Color.Opacity(Color.Green, 0.45f));
                        style.Line = new Pen(Color.Green, 2.0);
                        style.Outline = new Pen(Color.Green, 2.0);
                        break;
                    default:
                        style.Fill = new Brush(Color.Gray);
                        style.Outline = new Pen(Color.FromArgb(0, 64, 64, 64));
                        break;
                }

                return style;
            });

            return stl;
        }

        private static IStyle CreateFootprintPreviewThemeStyle()
        {
            var stl0 = new ThemeStyle(f =>
            {
                var style0 = new SymbolStyle()
                {
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.5,
                    MinVisible = 10000,
                    MaxVisible = double.MaxValue,
                };

                switch (f["State"].ToString())
                {
                    case "Unselect":
                        style0.Fill = new Brush(Color.Opacity(Color.Green, 0.25f));
                        style0.Line = new Pen(Color.Green, 1.0);
                        style0.Outline = new Pen(Color.Green, 1.0);
                        break;
                    case "Select":
                        style0.Fill = new Brush(Color.Opacity(Color.Red, 1.0f));
                        style0.Line = new Pen(Color.Green, 2.0);
                        style0.Outline = new Pen(Color.Green, 2.0);
                        break;
                    default:
                        style0.Fill = new Brush(Color.Gray);
                        style0.Outline = new Pen(Color.FromArgb(0, 64, 64, 64));
                        break;
                }

                return style0;
            });

            return stl0;
        }
    }
}
