using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Layers
{
    public class TargetLayer : MemoryLayer
    {
        private IStyle _style;
        private TargetProvider _provider;

        public TargetLayer(IProvider provider)
        {
            _provider = (TargetProvider)provider;
            _style = CreateTargetThemeStyle();
            
            Name = nameof(LayerType.GroundTarget);
            Style = _style;
            DataSource = provider;
            IsMapInfoLayer = false;
        }

        private static ThemeStyle CreateTargetThemeStyle()
        {
            return new ThemeStyle(f =>
            {
                if (f.Geometry is Point)
                {
                    return new SymbolStyle
                    {
                        Fill = new Brush(Color.Opacity(Color.Black, 0.3f)),
                        Outline = new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                        MaxVisible = 5000,
                    };
                }

                if (f.Geometry is LineString || f.Geometry is MultiLineString)
                {
                    return new VectorStyle
                    {
                        Fill = null,
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = 5000,
                    };
                }

                if (f.Geometry is Polygon || f.Geometry is MultiPolygon)
                {
                    return new VectorStyle
                    {
                        Fill = null,// new Brush(Color.Opacity(Color.Black, 0.3f)),
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = 5000,
                    };
                }

                throw new Exception();
            });
        }
    }
}
