using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Styles
{
    public class LayerStyleManager
    {
        private IStyle? _footprintStyle;
        private IStyle? _targetStyle;
        private IStyle? _sensorStyle;
        private IStyle? _trackStyle;
        private IStyle? _vertexOnlyStyle;
        private IStyle? _editStyle;
        private IStyle? _userStyle;
        private IStyle? _footprintImageBorderStyle;
        private IStyle? _designerStyle;
        private IStyle? _decoratorStyle;
        private IStyle? _selectStyle;
        private IStyle? _groundStationStyle;

        private const int _maxVisibleTargetStyle = 5000;
        private const int _maxVisibleFootprintStyle = 10000;

        private static readonly ColorPalette _satellitePalette = ColorPalette.DefaultPalette;

        private static readonly SingleHuePalette _groundStationPalette = SingleHuePalette.Purples;

        public LayerStyleManager() { }

        public static ColorPalette SatellitePalette => _satellitePalette;

        public static SingleHuePalette GroundStationPalette => _groundStationPalette;

        public IStyle FootprintStyle => _footprintStyle ??= CreateFootprintLayerStyle();

        public IStyle TargetStyle => _targetStyle ??= CreateTargetLayerStyle();

        public IStyle SensorStyle => _sensorStyle ??= CreateSensorStyle();

        public IStyle TrackStyle => _trackStyle ??= CreateTrackStyle();

        public IStyle VertexOnlyStyle => _vertexOnlyStyle ??= CreateVertexOnlyStyle();

        public IStyle EditStyle => _editStyle ??= CreateEditLayerStyle();

        public IStyle UserStyle => _userStyle ??= CreateUserLayerStyle();

        public IStyle FootprintImageBorderStyle => _footprintImageBorderStyle ??= CreateFootprintImageBorderStyle();

        public IStyle DesignerStyle => _designerStyle ??= CreateInteractiveLayerDesignerStyle();

        public IStyle DecoratorStyle => _decoratorStyle ??= CreateInteractiveLayerDecoratorStyle();

        public IStyle SelectStyle => _selectStyle ??= CreateInteractiveSelectLayerStyle();

        public IStyle GroundStationStyle => _groundStationStyle ??= CreateGroundStationLayerStyle();

        public int MaxVisibleFootprintStyle => _maxVisibleFootprintStyle;

        public int MaxVisibleTargetStyle => _maxVisibleTargetStyle;

        private static IStyle CreateFootprintLayerStyle()
        {
            //var style1 = CreateFootprintPreviewThemeStyle();
            var style2 = CreateFootprintThemeStyle();

            return new StyleCollection() { /*style1,*/ style2 };
        }

        private static IStyle CreateFootprintThemeStyle()
        {
            var stl = new ThemeStyle(f =>
            {
                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                if (gf.Geometry is Point)
                {
                    return null;
                }

                var style = new VectorStyle()
                {
                    Fill = new Brush(Color.Opacity(Color.Green, 0.25f)),
                    Line = new Pen(Color.Green, 1.0),
                    Outline = new Pen(Color.Green, 1.0),
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                };

                if (gf.Fields.Contains("Interactive.Select"))
                {
                    if ((bool)gf["Interactive.Select"]! == true)
                    {
                        style.Fill = new Brush(Color.Opacity(Color.Green, 0.55f));
                        style.Outline = new Pen(Color.Black, 4.0);
                        style.Line = new Pen(Color.Black, 4.0);

                        return style;
                    }
                }

                return style;
            });

            return stl;
        }

        //private static IStyle CreateFootprintPreviewThemeStyle()
        //{
        //    var stl0 = new ThemeStyle(f =>
        //    {
        //        var style0 = new SymbolStyle()
        //        {
        //            SymbolType = SymbolType.Ellipse,
        //            SymbolScale = 0.2,
        //            MinVisible = _maxVisibleFootprintStyle,
        //            MaxVisible = double.MaxValue,
        //        };

        //        switch (f["State"].ToString())
        //        {
        //            case "Unselect":
        //                style0.Fill = new Brush(Color.Opacity(Color.Green, 0.25f));
        //                style0.Line = new Pen(Color.Green, 1.0);
        //                style0.Outline = new Pen(Color.Green, 1.0);
        //                break;
        //            case "Select":
        //                style0.Fill = new Brush(Color.Opacity(Color.Green, 1.0f));
        //                style0.Line = new Pen(Color.Black, 4.0);
        //                style0.Outline = new Pen(Color.Black, 4.0);
        //                break;
        //            default:
        //                style0.Fill = new Brush(Color.Gray);
        //                style0.Outline = new Pen(Color.FromArgb(0, 64, 64, 64));
        //                break;
        //        }

        //        return style0;
        //    });

        //    return stl0;
        //}

        private static IStyle CreateTargetLayerStyle()
        {
            var style1 = CreateTargetHighlightThemeStyle();
            var style2 = CreateTargetThemeStyle();

            return new StyleCollection() { style1, style2 };
        }

        private static ThemeStyle CreateTargetHighlightThemeStyle()
        {
            return new ThemeStyle(f =>
            {
                var highlight = (bool)f["Highlight"]!;
                var state = f["State"]!.ToString();

                if (highlight == true)
                {
                    if (f is not GeometryFeature gf)
                    {
                        return null;
                    }

                    if (gf.Geometry is Point)
                    {
                        switch (state)
                        {
                            case "Unselected":
                                return new SymbolStyle
                                {
                                    Fill = new Brush(Color.Opacity(Color.Green, 0.4f)),
                                    Outline = null,
                                    SymbolType = SymbolType.Ellipse,
                                    SymbolScale = 0.6,
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            case "Selected":
                                return new SymbolStyle
                                {
                                    Fill = new Brush(Color.Opacity(Color.Green, 0.4f)),
                                    Outline = null,
                                    SymbolType = SymbolType.Ellipse,
                                    SymbolScale = 0.8,
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            default:
                                break;
                        }
                    }

                    if (gf.Geometry is LineString || gf.Geometry is MultiLineString)
                    {
                        switch (state)
                        {
                            case "Unselected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(Color.Opacity(Color.Green, 0.3f), 8),
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            case "Selected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(Color.Opacity(Color.Green, 0.3f), 12),
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            default:
                                break;
                        }
                    }

                    if (gf.Geometry is Polygon || gf.Geometry is MultiPolygon)
                    {
                        switch (state)
                        {
                            case "Unselected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Outline = new Pen(Color.Opacity(Color.Green, 0.3f), 8),
                                    Line = null,
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            case "Selected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Outline = new Pen(Color.Opacity(Color.Green, 0.3f), 12),
                                    Line = null,
                                    MaxVisible = _maxVisibleTargetStyle,
                                };
                            default:
                                break;
                        }
                    }

                    throw new Exception();
                }

                return null;
            });
        }

        private static ThemeStyle CreateTargetThemeStyle()
        {
            return new ThemeStyle(f =>
            {
                var state = f["State"]!.ToString();

                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                var style = new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleTargetStyle,
                };

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Interactive.Select") == true)
                        {
                            var isSelect = (bool)gf["Interactive.Select"]!;

                            if (isSelect == true)
                            {
                                if (gf.Geometry is Point)
                                {
                                    return new SymbolStyle
                                    {
                                        Fill = new Brush(Color.Opacity(Color.Black, 0.55f)),
                                        Outline = new Pen(Color.Black, 4.0),
                                        SymbolType = SymbolType.Ellipse,
                                        SymbolScale = 0.6,
                                        MaxVisible = _maxVisibleTargetStyle,
                                    };
                                }

                                style.Fill = new Brush(Color.Opacity(Color.Black, 0.55f));
                                style.Outline = new Pen(Color.Black, 4.0);
                                style.Line = new Pen(Color.Black, 4.0);
                                return style;
                            }
                        }
                    }
                }

                if (gf.Geometry is Point)
                {
                    switch (state)
                    {
                        case "Unselected":
                            return new SymbolStyle
                            {
                                Fill = new Brush(Color.Opacity(Color.Black, 0.25f)),
                                Outline = new Pen(Color.Black, 1.0),
                                SymbolType = SymbolType.Ellipse,
                                SymbolScale = 0.4,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        case "Selected":
                            return new SymbolStyle
                            {
                                Fill = new Brush(Color.Opacity(Color.Black, 0.25f)),
                                Outline = new Pen(Color.Black, 4.0),
                                SymbolType = SymbolType.Ellipse,
                                SymbolScale = 0.6,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        default:
                            break;
                    }
                }

                if (gf.Geometry is LineString || gf.Geometry is MultiLineString)
                {
                    switch (state)
                    {
                        case "Unselected":
                            style.Fill = null;
                            style.Line = new Pen(Color.Black, 2);
                            break;
                        case "Selected":
                            style.Fill = null;
                            style.Line = new Pen(Color.Black, 4);
                            break;
                        default:
                            break;
                    }
                }

                if (gf.Geometry is Polygon || gf.Geometry is MultiPolygon)
                {
                    switch (state)
                    {
                        case "Unselected":
                            style.Fill = new Brush(Color.Opacity(Color.Black, 0.25f));
                            style.Outline = new Pen(Color.Black, 1);
                            style.Line = null;
                            break;
                        case "Selected":
                            style.Fill = new Brush(Color.Opacity(Color.Black, 0.25f));
                            style.Outline = new Pen(Color.Black, 4);
                            style.Line = null;
                            break;
                        default:
                            break;
                    }
                }

                return style;
            });
        }

        private static IStyle CreateSensorStyle()
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

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Name") == true)
                        {
                            var name = (string)gf["Name"]!;

                            var color = _satellitePalette.PickColor(name);

                            return new VectorStyle
                            {
                                Fill = new Brush(Color.FromArgb(75, color.R, color.G, color.B)),
                                Line = null,
                                Outline = null,
                            };
                        }
                    }
                }

                var defaultStyle = new VectorStyle
                {
                    Fill = new Brush(Color.Opacity(Color.Black, 0.25f)),
                    Line = null,
                    Outline = null,
                };

                return defaultStyle;
            });
        }

        private static IStyle CreateTrackStyle()
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

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Name") == true)
                        {
                            var name = (string)gf["Name"]!;

                            var color = _satellitePalette.PickColor(name);

                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(Color.FromArgb(255, color.R, color.G, color.B), 2),
                            };
                        }
                    }
                }

                var defaultStyle = new VectorStyle
                {
                    Fill = null,
                    Line = new Pen(Color.Black, 1),
                };

                return defaultStyle;
            });
        }

        private static IStyle CreateVertexOnlyStyle()
        {
            return new SymbolStyle
            {
                Fill = new Brush(Color.White),
                Outline = new Pen(Color.Black, 2 / 0.3),
                SymbolType = SymbolType.Ellipse,
                SymbolScale = 0.3,
            };
        }

        private static IStyle CreateEditLayerStyle()
        {
            // To show the selected style a ThemeStyle is used which switches on and off the SelectedStyle
            // depending on a "Selected" attribute.
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

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Name"))
                        {
                            return FeatureStyles.Get((string)gf["Name"]!);
                        }
                    }
                }

                if (gf.Geometry is Polygon)
                {
                    var editModeColor = new Color(124, 22, 111, 180);

                    return new VectorStyle
                    {
                        Fill = new Brush(editModeColor),
                        Line = new Pen(editModeColor, 3),
                        Outline = new Pen(editModeColor, 3)
                    };
                }

                return null;
            });
        }

        private static IStyle CreateUserLayerStyle()
        {
            return new ThemeStyle(f =>
            {
                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                var style = new VectorStyle();

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Interactive.Select") == true)
                        {
                            var isSelect = (bool)gf["Interactive.Select"]!;

                            if (isSelect == true)
                            {
                                if (gf.Geometry is Point)
                                {
                                    return new SymbolStyle
                                    {
                                        Fill = new Brush(Color.Red),
                                        Outline = new Pen(Color.Black, 2 / 0.3),
                                        SymbolType = SymbolType.Ellipse,
                                        SymbolScale = 0.8,
                                    };
                                }

                                style.Fill = new Brush(new Color(new Color(20, 120, 120, 40)));
                                style.Outline = new Pen(new Color(20, 120, 120), 4.0);
                                style.Line = new Pen(new Color(20, 20, 20), 4.0);
                                return style;
                            }
                        }
                    }
                }

                if (gf.Geometry is Point)
                {
                    return new SymbolStyle()
                    {
                        Fill = new Brush(Color.Red),
                        Outline = new Pen(Color.Black, 2 / 0.3),
                        Line = null,//new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.5,
                    };
                }
                else
                {
                    var backgroundColor = new Color(20, 120, 120, 40);
                    var lineColor = new Color(20, 120, 120);
                    var outlineColor = new Color(20, 20, 20);

                    style.Fill = new Brush(new Color(backgroundColor));
                    style.Line = new Pen(lineColor, 3);
                    style.Outline = new Pen(outlineColor, 3);

                    return style;
                }
            });
        }

        private static IStyle CreateFootprintImageBorderStyle()
        {
            var color = new Color() { R = 76, G = 185, B = 247, A = 255 };

            return new VectorStyle
            {
                Fill = new Brush(Color.Opacity(color, 0.25f)),
                Line = new Pen(color, 1.0),
                Outline = new Pen(color, 1.0),
                Enabled = true
            };
        }

        private static IStyle CreateInteractiveLayerDesignerStyle()
        {
            // To show the selected style a ThemeStyle is used which switches on and off the SelectedStyle
            // depending on a "Selected" attribute.
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
                        Fill = new Brush(Color.White),
                        Outline = new Pen(Color.Black, 2 / 0.3),
                        Line = null,//new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                    };
                }

                var _color = new Color(76, 154, 231);
                //Color _darkColor = new Color(67, 135, 202);

                if (gf.Fields != null)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Name") == true)
                        {
                            if ((string)gf["Name"]! == "ExtraPolygonHoverLine")
                            {
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(_color, 4) { PenStyle = PenStyle.Dot },
                                };
                            }
                            else if ((string)gf["Name"]! == "ExtraPolygonArea")
                            {
                                return new VectorStyle
                                {
                                    Fill = new Brush(Color.Opacity(_color, 0.25f)),
                                    Line = null,
                                    Outline = null,
                                };
                            }
                            else if ((string)gf["Name"]! == "ExtraRouteHoverLine")
                            {
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(_color, 3) { PenStyle = PenStyle.Dash },
                                };
                            }
                        }
                    }
                }

                if (gf.Geometry is Polygon || gf.Geometry is LineString)
                {
                    return new VectorStyle
                    {
                        Fill = new Brush(Color.Transparent),
                        Line = new Pen(_color, 3),
                        Outline = new Pen(_color, 3)
                    };
                }

                return null;
            });
        }

        private static IStyle CreateInteractiveLayerDecoratorStyle()
        {
            // To show the selected style a ThemeStyle is used which switches on and off the SelectedStyle
            // depending on a "Selected" attribute.
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
                        Fill = new Brush(Color.White),
                        Outline = new Pen(Color.Black, 2 / 0.3),
                        Line = null,//new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.3,
                    };
                }

                return null;
            });
        }

        private static IStyle CreateInteractiveSelectLayerStyle()
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
                        Fill = new Brush(Color.Red),
                        Outline = new Pen(Color.Green, 2 / 0.3),
                        Line = null,//new Pen(Color.Black, 2),
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = 0.9,
                    };
                }
                else
                {
                    return new VectorStyle()
                    {
                        Fill = new Brush(Color.Transparent),
                        Outline = new Pen(Color.Green, 4),
                        Line = new Pen(Color.Green, 4),
                    };
                }
            });
        }

        private static IStyle CreateGroundStationLayerStyle()
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

                if (gf.Geometry is MultiPolygon)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("Count"))
                        {
                            var count = int.Parse(gf["Count"]!.ToString()!);
                            var index = int.Parse(gf["Index"]!.ToString()!);
                            var color = _groundStationPalette.GetColor(index, count);
                            return new VectorStyle
                            {
                                Fill = new Brush(Color.Opacity(new Color(color.R, color.G, color.B), 0.45f)),
                                Line = null,
                                Outline = null,
                                Enabled = true
                            };
                        }
                    }
                }

                if (gf.Fields != null && gf.Geometry is MultiLineString)
                {
                    foreach (var item in gf.Fields)
                    {
                        if (item.Equals("InnerBorder"))
                        {
                            var count = int.Parse(gf["Count"]!.ToString()!);
                            var color = _groundStationPalette.GetColor(0, count);

                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(new Color(color.R, color.G, color.B), 2.0),
                                Outline = new Pen(new Color(color.R, color.G, color.B), 2.0),
                                Enabled = true
                            };
                        }
                        if (item.Equals("OuterBorder"))
                        {
                            var count = int.Parse(gf["Count"]!.ToString()!);
                            var color = _groundStationPalette.GetColor(count - 1, count);

                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(new Color(color.R, color.G, color.B), 2.0),
                                Outline = new Pen(new Color(color.R, color.G, color.B), 2.0),
                                Enabled = true
                            };
                        }
                    }
                }

                return null;
            });
        }
    }

    public enum FeatureType
    {
        Route,
        AOIRectangle,
        AOIPolygon,
        AOICircle,
    }

    internal static class FeatureStyles
    {
        private static readonly Dictionary<string, VectorStyle> _dict;
        private static readonly Color _color = new() { R = 76, G = 154, B = 231, A = 255 };
        private static readonly Pen _pen = new(_color, 3);
        private static readonly Color _darkColor = new() { R = 67, G = 135, B = 202, A = 255 };
        private static readonly Pen _darkPen = new(_darkColor, 3);

        static FeatureStyles()
        {
            _dict = new Dictionary<string, VectorStyle>()
            {
                { FeatureType.Route.ToString(), RouteStyle },
                { FeatureType.AOIRectangle.ToString(), AOIRectangleStyle },
                { FeatureType.AOIPolygon.ToString(), AOIPolygonStyle },
                { FeatureType.AOICircle.ToString(), AOICircleStyle },
            };
        }

        public static VectorStyle? Get(string feature)
        {
            return _dict.TryGetValue(feature, out var style) ? style : null;
        }

        private static readonly VectorStyle RouteStyle = new()
        {
            Fill = null,
            Line = _darkPen
        };

        private static readonly VectorStyle AOIRectangleStyle = new()
        {
            Fill = null,
            Outline = _pen,
        };

        private static readonly VectorStyle AOIPolygonStyle = new()
        {
            Fill = null,
            Line = _pen,
            Outline = _pen,
        };

        private static readonly VectorStyle AOICircleStyle = new()
        {
            Fill = null,
            Outline = _pen,
        };
    }
}
