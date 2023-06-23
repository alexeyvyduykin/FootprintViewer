using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;

namespace PlannedScheduleOnMapSample.Models;

public static class StyleBuilder
{
    private const int _maxVisibleFootprintStyle = 10000;

    public static IStyle CreateFootprintStyle(bool dimming)
    {
        return new VectorStyle()
        {
            Fill = new Brush(Color.Opacity(Color.Green, 0.25f)),
            Line = new Pen(Color.Green, 1.0),
            Outline = new Pen(Color.Green, 1.0),
            MinVisible = 0,
            MaxVisible = _maxVisibleFootprintStyle,
            Opacity = (dimming) ? 0.3f : 1.0f
        };
    }

    public static IStyle CreateSelectFootprintStyle()
    {
        return new VectorStyle()
        {
            MinVisible = 0,
            MaxVisible = _maxVisibleFootprintStyle,
            Fill = new Brush(Color.Opacity(Color.Green, 0.55f)),
            Outline = new Pen(Color.Black, 4.0),
            Line = new Pen(Color.Black, 4.0)
        };
    }

    public static IStyle CreateHoverFootprintStyle()
    {
        return new VectorStyle()
        {
            MinVisible = 0,
            MaxVisible = _maxVisibleFootprintStyle,
            Fill = new Brush(Color.Opacity(Color.Green, 0.85f)),
            Outline = new Pen(Color.Yellow, 3.0),
            Line = new Pen(Color.Yellow, 3.0)
        };
    }

    public static IStyle CreateTrackLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            if (gf.Geometry is Point)
            {
                return new SymbolStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Black, 0.55f)),
                    Line = new Pen(Color.Black, 1.0),
                    Outline = new Pen(Color.Black, 1.0),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.8,
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                };
                // return null;
            }

            if ((string)gf["Name"]! == "Target")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Blue, 1.0f)),
                    // Outline = new Pen(Color.Blue, 2.0),
                    Line = new Pen(Color.Opacity(Color.Black, 1.0f), 2.0)
                };
            }

            if ((string)gf["Name"]! == "FootprintTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Blue, 1.0f)),
                    // Outline = new Pen(Color.Blue, 2.0),
                    Line = new Pen(Color.Opacity(Color.Blue, 0.65f), 12.0)
                };
            }

            if ((string)gf["Name"]! == "BaseTrack")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    //  Fill = new Brush(Color.Opacity(Color.Black, 0.55f)),
                    //  Outline = new Pen(Color.Black, 1.0),
                    Line = new Pen(Color.Opacity(Color.Black, 0.20f), 12.0)
                };
            }

            if ((string)gf["Name"]! == "FootprintSwath")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Orange, 1.0f)),
                    Outline = new Pen(Color.Orange, 2.0),
                    Line = new Pen(Color.Orange, 2.0)
                };
            }

            if ((string)gf["Name"]! == "BaseSwath")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    // Fill = new Brush(Color.Opacity(Color.Indigo, 0.55f)),
                    Outline = new Pen(Color.Orange, 2.0),
                    Line = new Pen(Color.Orange, 2.0)
                };
            }

            if ((string)gf["Name"]! == "Arrow")
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Red, 1.0f)),
                    Outline = new Pen(Color.Red, 1.0),
                    Line = new Pen(Color.Red, 1.0)
                };
            }

            if ((string)gf["Name"]! == "AreaPoly")
            {
                return new VectorStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Orange, 0.25f)),
                    Line = null,
                    Outline = null,
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                };
            }

            return new VectorStyle()
            {
                Fill = null,// new Brush(Color.Opacity(Color.Green, 0.55f)),
                Line = new Pen(Color.Black, 2.0),
                Outline = new Pen(Color.Black, 2.0),
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
            };
        });
    }

    public static IStyle CreateGroundTargetLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            bool isSelect = gf["Select"] is true;

            var width = isSelect ? 4.0f : 2.0f;

            if (gf.Geometry is Point)
            {
                return new SymbolStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Black, width),
                    Line = new Pen(Color.Black, width),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.4,
                };
            }

            if ((string)gf["Type"]! == "Route")
            {
                var styleBorder = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Opacity(Color.Black, 0.05f), 16.0),
                    Line = new Pen(Color.Opacity(Color.Black, 0.05f), 16.0)
                };

                var style = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                    Outline = new Pen(Color.Black, width),
                    Line = new Pen(Color.Black, width)
                };

                return new StyleCollection
                {
                    Styles = new() { styleBorder, style }
                };
            }

            return new VectorStyle()
            {
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
                Fill = new Brush(Color.Opacity(Color.Black, 0.05f)),
                Outline = new Pen(Color.Black, width),
                Line = new Pen(Color.Black, width)
            };
        });
    }

    public static IStyle CreateSatelliteLayerStyle()
    {
        return new ThemeStyle(f =>
        {
            if (f is not GeometryFeature gf)
            {
                return null;
            }

            if (gf.Geometry is Point)
            {
                return null;
            }

            if ((string)gf["Name"]! == "FootprintTrack")
            {
                return new VectorStyle()
                {
                    Line = new Pen(Color.Opacity(Color.Red, 0.65f), 12.0)
                };
            }

            return new VectorStyle()
            {
                Line = new Pen(Color.Opacity(Color.Green, 0.25f), 12.0),
            };
        });
    }
}
