using Mapsui.Geometries;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Layers
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

        private const int _maxVisibleTargetStyle = 5000;
        private const int _maxVisibleFootprintStyle = 10000;

        public LayerStyleManager() { }

        public IStyle FootprintStyle => _footprintStyle ??= CreateFootprintLayerStyle();

        public IStyle TargetStyle => _targetStyle ??= CreateTargetLayerStyle();

        public IStyle SensorStyle => _sensorStyle ??= CreateSensorStyle();

        public IStyle TrackStyle => _trackStyle ??= CreateTrackStyle();

        public IStyle VertexOnlyStyle => _vertexOnlyStyle ??= CreateVertexOnlyStyle();

        public IStyle EditStyle => _editStyle ??= CreateEditLayerStyle();

        public IStyle UserStyle => _userStyle ??= CreateUserLayerStyle();

        public IStyle FootprintImageBorderStyle => _footprintImageBorderStyle ??= CreateFootprintImageBorderStyle();

        public int MaxVisibleFootprintStyle => _maxVisibleFootprintStyle;

        public int MaxVisibleTargetStyle => _maxVisibleTargetStyle;

        private static IStyle CreateFootprintLayerStyle()
        {
            var style1 = CreateFootprintPreviewThemeStyle();
            var style2 = CreateFootprintThemeStyle();

            return new StyleCollection() { style1, style2 };
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
                    MaxVisible = _maxVisibleFootprintStyle,
                };

                switch (f["State"].ToString())
                {
                    case "Unselect":
                        style.Fill = new Brush(Color.Opacity(Color.Green, 0.25f));
                        style.Line = new Pen(Color.Green, 1.0);
                        style.Outline = new Pen(Color.Green, 1.0);
                        break;
                    case "Select":
                        style.Fill = new Brush(Color.Opacity(Color.Green, 0.55f));
                        style.Line = new Pen(Color.Black, 4.0);
                        style.Outline = new Pen(Color.Black, 4.0);
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
                    MinVisible = _maxVisibleFootprintStyle,
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
                        style0.Fill = new Brush(Color.Opacity(Color.Green, 1.0f));
                        style0.Line = new Pen(Color.Black, 4.0);
                        style0.Outline = new Pen(Color.Black, 4.0);
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
                var highlight = (bool)f["Highlight"];
                var state = f["State"].ToString();

                if (highlight == true)
                {
                    if (f.Geometry is Point)
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

                    if (f.Geometry is LineString || f.Geometry is MultiLineString)
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

                    if (f.Geometry is Polygon || f.Geometry is MultiPolygon)
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
                var state = f["State"].ToString();

                if (f.Geometry is Point)
                {
                    switch (state)
                    {
                        case "Unselected":
                            return new SymbolStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 1.0),
                                SymbolType = SymbolType.Ellipse,
                                SymbolScale = 0.4,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        case "Selected":
                            return new SymbolStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 4.0),
                                SymbolType = SymbolType.Ellipse,
                                SymbolScale = 0.6,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        default:
                            break;
                    }
                }

                if (f.Geometry is LineString || f.Geometry is MultiLineString)
                {
                    switch (state)
                    {
                        case "Unselected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(Color.Black, 1),
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        case "Selected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(Color.Black, 4),
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        default:
                            break;
                    }
                }

                if (f.Geometry is Polygon || f.Geometry is MultiPolygon)
                {
                    switch (state)
                    {
                        case "Unselected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 1),
                                Line = null,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        case "Selected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 4),
                                Line = null,
                                MaxVisible = _maxVisibleTargetStyle,
                            };
                        default:
                            break;
                    }
                }

                throw new Exception();
            });
        }

        private static IStyle CreateSensorStyle()
        {
            var stl = new VectorStyle
            {
                Fill = new Brush(Color.Opacity(Color.Blue, 0.25f)),
                Line = null,
                Outline = null,
            };

            return stl;
        }

        private static IStyle CreateTrackStyle()
        {
            var stl = new VectorStyle
            {
                Fill = null,
                Line = new Pen(Color.Green, 1),
            };

            return stl;
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
                if (f.Geometry is Point)
                {
                    return null;
                }

                if (f.Fields != null)
                {
                    foreach (var item in f.Fields)
                    {
                        if (item.Equals("Name"))
                        {
                            return FeatureStyles.Get((string)f["Name"]);
                        }
                    }
                }

                if (f.Geometry is Polygon)
                {
                    Color editModeColor = new Color(124, 22, 111, 180);

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
            Color backgroundColor = new Color(20, 120, 120, 40);
            Color lineColor = new Color(20, 120, 120);
            Color outlineColor = new Color(20, 20, 20);

            return new VectorStyle
            {
                Fill = new Brush(new Color(backgroundColor)),
                Line = new Pen(lineColor, 3),
                Outline = new Pen(outlineColor, 3)
            };
        }

        private static IStyle CreateFootprintImageBorderStyle()
        {
            Color color = new Color() { R = 76, G = 185, B = 247, A = 255 };

            return new VectorStyle
            {
                Fill = new Brush(Color.Opacity(color, 0.25f)),
                Line = new Pen(color, 1.0),
                Outline = new Pen(color, 1.0),
                Enabled = true
            };
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
        private static readonly Color _color = new Color { R = 76, G = 154, B = 231, A = 255 };
        private static readonly Pen _pen = new Pen(_color, 3);
        private static readonly Pen _penDot = new Pen(_color, 4) { PenStyle = PenStyle.Dot };
        private static readonly Brush _brush = new Brush(Color.Opacity(_color, 0.25f));
        private static readonly Color _darkColor = new Color { R = 67, G = 135, B = 202, A = 255 };
        private static readonly Pen _darkPen = new Pen(_darkColor, 3);
        private static readonly Pen _darkPenDash = new Pen(_darkColor, 3) { PenStyle = PenStyle.Dash };

        static FeatureStyles()
        {
            _dict = new Dictionary<string, VectorStyle>()
            {
                { FeatureType.Route.ToString(), RouteStyle },
              //  { FeatureType.RouteDrawing.ToString(), RouteDrawingStyle },
                { FeatureType.AOIRectangle.ToString(), AOIRectangleStyle },
              //  { FeatureType.AOIRectangleDrawing.ToString(), AOIRectangleDrawingStyle },
                { FeatureType.AOIPolygon.ToString(), AOIPolygonStyle },
              //  { FeatureType.AOIPolygonBorderDrawing.ToString(), AOIPolygonBorderStyle },
              //  { FeatureType.AOIPolygonAreaDrawing.ToString(), AOIPolygonAreaStyle },
              //  { FeatureType.AOIPolygonDrawing.ToString(), AOIPolygonDrawingStyle },
                { FeatureType.AOICircle.ToString(), AOICircleStyle },
              //  { FeatureType.AOICircleDrawing.ToString(), AOICircleDrawingStyle },
            };
        }

        public static VectorStyle? Get(string feature)
        {
            return _dict.TryGetValue(feature, out var style) ? style : null;
        }

        private static readonly VectorStyle RouteStyle = new VectorStyle
        {
            Fill = null,
            Line = _darkPen
        };

        private static readonly VectorStyle AOIRectangleStyle = new VectorStyle
        {
            Fill = null,
            Outline = _pen,
        };

        private static readonly VectorStyle AOIPolygonStyle = new VectorStyle
        {
            Fill = null,
            Line = _pen,
            Outline = _pen,
        };

        private static readonly VectorStyle AOICircleStyle = new VectorStyle
        {
            Fill = null,
            Outline = _pen,
        };

        private static readonly VectorStyle RouteDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = _darkPenDash,
        };

        private static readonly VectorStyle AOIRectangleDrawingStyle = new VectorStyle
        {
            Fill = _brush,
            Outline = _pen,
        };

        private static readonly VectorStyle AOIPolygonBorderStyle = new VectorStyle
        {
            Fill = null,
            Line = _pen,
        };

        private static readonly VectorStyle AOIPolygonAreaStyle = new VectorStyle
        {
            Fill = _brush,
            Line = null,
            Outline = null,
        };

        private static readonly VectorStyle AOIPolygonDrawingStyle = new VectorStyle
        {
            Fill = null,
            Line = _penDot,
        };

        private static readonly VectorStyle AOICircleDrawingStyle = new VectorStyle
        {
            Fill = _brush,
            Outline = _pen,
        };
    }
}
