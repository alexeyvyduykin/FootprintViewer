using FootprintViewer.Models;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Styles;

public class LayerStyleManager
{
    private IStyle? _designerStyle;
    private IStyle? _decoratorStyle;
    private IStyle? _selectStyle;

    private const int _maxVisibleTargetStyle = 5000;
    private const int _maxVisibleFootprintStyle = 10000;

    // TODO: interactivity keys refactoring
    private const string SelectField = "Select";// Mapsui.Interactivity.InteractiveFields.Select;
    private const string HoverField = "Highlight";

    private const string ExtraPolygonHoverLineField = "ExtraPolygonHoverLine";// Mapsui.Interactivity.InteractiveNames.ExtraPolygonHoverLine;
    private const string ExtraPolygonAreaField = "ExtraPolygonArea";// Mapsui.Interactivity.InteractiveNames.ExtraPolygonArea;
    private const string ExtraRouteHoverLineField = "ExtraRouteHoverLine";// Mapsui.Interactivity.InteractiveNames.ExtraRouteHoverLine;

    private readonly Dictionary<LayerType, LayerStyleViewModel[]> _dict;
    private readonly Dictionary<LayerType, LayerStyleViewModel?> _selectedDict;

    public LayerStyleManager()
    {
        var fotprintImageBorderPalette = new MapsuiPalette(new Color() { R = 76, G = 185, B = 247, A = 255 });
        var defaultPalette = ColorPalette.DefaultPalette;
        var springPastels = ColorPalette.SpringPastelsPalette;
        var dutchField = ColorPalette.DutchFieldPalette;
        var riverNights = ColorPalette.RiverNightsPalette;
        var retroMetro = ColorPalette.RetroMetroPalette;

        _dict = new()
        {
            { LayerType.Footprint, new[] { new LayerStyleViewModel("Footprint", "Light", new MapsuiPalette(Color.Green), p => CreateFootprintLayerStyle(p)) } },
            { LayerType.GroundTarget, new[] { new LayerStyleViewModel("GroundTarget", "Light", null, p => CreateTargetLayerStyle(p)) } },
            { LayerType.Track, new[]
            {
                new LayerStyleViewModel("Default", "Light", defaultPalette, p => CreateTrackStyle(p)),
                new LayerStyleViewModel("SpringPastels", "Light", springPastels, p => CreateTrackStyle(p)),
                new LayerStyleViewModel("DutchField", "Light", dutchField, p => CreateTrackStyle(p)),
                new LayerStyleViewModel("RiverNights", "Light", riverNights, p => CreateTrackStyle(p)),
                new LayerStyleViewModel("RetroMetro", "Light", retroMetro, p => CreateTrackStyle(p)),
            } },
            { LayerType.Sensor, new[]
            {
                new LayerStyleViewModel("Default", "Light", defaultPalette, p => CreateSensorStyle(p)),
                new LayerStyleViewModel("SpringPastels", "Light", springPastels, p => CreateSensorStyle(p)),
                new LayerStyleViewModel("DutchField", "Light", dutchField, p => CreateSensorStyle(p)),
                new LayerStyleViewModel("RiverNights", "Light", riverNights, p => CreateSensorStyle(p)),
                new LayerStyleViewModel("RetroMetro", "Light", retroMetro, p => CreateSensorStyle(p)),
            } },
            { LayerType.User, new[] { new LayerStyleViewModel("User", "Light", null, p => CreateUserLayerStyle(p)) } },
            { LayerType.Edit, new[] { new LayerStyleViewModel("Edit", "Light", null, p => CreateEditLayerStyle(p)) } },
            { LayerType.GroundStation, new[]
            {
                new LayerStyleViewModel("Purple", "Light", SingleHuePalette.Purples, p => CreateGroundStationLayerStyle(p)),
                new LayerStyleViewModel("Green", "Light", SingleHuePalette.Greens, p => CreateGroundStationLayerStyle(p)),
                new LayerStyleViewModel("Blue", "Light", SingleHuePalette.Blues, p => CreateGroundStationLayerStyle(p)),
                new LayerStyleViewModel("Grey", "Light", SingleHuePalette.Greys, p => CreateGroundStationLayerStyle(p)),
                new LayerStyleViewModel("Orange", "Light", SingleHuePalette.Oranges, p => CreateGroundStationLayerStyle(p)),
                new LayerStyleViewModel("Red", "Light", SingleHuePalette.Reds, p => CreateGroundStationLayerStyle(p)),
            } },
            { LayerType.Vertex, new[] { new LayerStyleViewModel("Vertex", "Light", null, p => CreateVertexOnlyStyle()) } },
            { LayerType.FootprintImageBorder, new[] { new LayerStyleViewModel("FootprintBorder", "Light", fotprintImageBorderPalette, p => CreateFootprintImageBorderStyle(p)) } },
        };

        _selectedDict = _dict.ToDictionary(s => s.Key, s => s.Value.FirstOrDefault());

        Selected = ReactiveCommand.Create<LayerType, (LayerType, LayerStyleViewModel?)>(s => (s, _selectedDict[s]), outputScheduler: RxApp.MainThreadScheduler);
    }

    public ReactiveCommand<LayerType, (LayerType, LayerStyleViewModel?)> Selected { get; }

    public IStyle? GetStyle(LayerType type)
    {
        if (_selectedDict.ContainsKey(type) == true)
        {
            return _selectedDict[type]?.GetStyle();
        }

        return null;
    }

    public LayerStyleViewModel? GetStyle(string layerName)
    {
        Enum.TryParse(typeof(LayerType), layerName, out var result);

        if (result is LayerType type)
        {
            if (_selectedDict.ContainsKey(type) == true)
            {
                return _selectedDict[type];
            }
        }

        return null;
    }

    public T? GetPalette<T>(LayerType type) where T : IPalette
    {
        if (_selectedDict.ContainsKey(type) == true)
        {
            var p = _selectedDict[type]?.Palette;

            if (p is T palette)
            {
                return palette;
            }
        }

        return default;
    }

    public LayerStyleViewModel[] GetStyles(string layerName)
    {
        Enum.TryParse(typeof(LayerType), layerName, out var result);

        if (result is LayerType type)
        {
            return _dict.ContainsKey(type) ? _dict[type] : Array.Empty<LayerStyleViewModel>();
        }

        return Array.Empty<LayerStyleViewModel>();
    }

    public void Select(ILayer layer, LayerStyleViewModel? selectedStyle = null)
    {
        var layerName = layer.Name;

        Enum.TryParse(typeof(LayerType), layerName, out var result);

        if (result is LayerType type)
        {
            if (selectedStyle == null)
            {
                layer.Style = GetStyle(type);
                return;
            }

            if (_dict.ContainsKey(type) == true)
            {
                var res = _dict[type].Where(s => Equals(s.Name, selectedStyle.Name)).FirstOrDefault();

                if (res != null)
                {
                    layer.Style = res.GetStyle();

                    _selectedDict[type] = res;

                    Selected.Execute(type).Subscribe();
                }
            }
        }
    }

    public IStyle DesignerStyle => _designerStyle ??= CreateInteractiveLayerDesignerStyle();

    public IStyle DecoratorStyle => _decoratorStyle ??= CreateInteractiveLayerDecoratorStyle();

    public IStyle SelectStyle => _selectStyle ??= CreateInteractiveSelectLayerStyle();

    public int MaxVisibleFootprintStyle => _maxVisibleFootprintStyle;

    public int MaxVisibleTargetStyle => _maxVisibleTargetStyle;

    private static IStyle CreateFootprintLayerStyle(IPalette? palette)
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

            if ((palette is IMapsuiPalette mapsuiPalette) == false)
            {
                return null;
            }

            if (gf[SelectField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.55f)),
                    Outline = new Pen(Color.Black, 4.0),
                    Line = new Pen(Color.Black, 4.0)
                };
            }

            if (gf[HoverField] is true)
            {
                return new VectorStyle()
                {
                    MinVisible = 0,
                    MaxVisible = _maxVisibleFootprintStyle,
                    Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.85f)),
                    Outline = new Pen(Color.Yellow, 3.0),
                    Line = new Pen(Color.Yellow, 3.0)
                };
            }

            return new VectorStyle()
            {
                Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.25f)),
                Line = new Pen(mapsuiPalette.Color, 1.0),
                Outline = new Pen(mapsuiPalette.Color, 1.0),
                MinVisible = 0,
                MaxVisible = _maxVisibleFootprintStyle,
            };
        });
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

    private static IStyle CreateTargetLayerStyle(IPalette? _)
    {
        var style1 = CreateTargetHighlightThemeStyle();
        var style2 = CreateTargetThemeStyle();

        return new StyleCollection()
        {
            Styles = { style1, style2 }
        };
    }

    private static ThemeStyle CreateTargetHighlightThemeStyle()
    {
        return new ThemeStyle(f =>
        {
            var highlight = (bool)f[HoverField]!;
            // TODO: fix this
            var isSelected = (bool?)f[SelectField] ?? false;

            if (highlight == true)
            {
                if (f is not GeometryFeature gf)
                {
                    return null;
                }

                if (gf.Geometry is Point)
                {
                    return new SymbolStyle
                    {
                        Fill = new Brush(Color.Opacity(Color.Green, 0.4f)),
                        Outline = null,
                        SymbolType = SymbolType.Ellipse,
                        SymbolScale = (isSelected == true) ? 0.8 : 0.6,
                        MaxVisible = _maxVisibleTargetStyle,
                    };
                }

                if (gf.Geometry is LineString || gf.Geometry is MultiLineString)
                {
                    return new VectorStyle
                    {
                        Fill = null,
                        Line = new Pen(Color.Opacity(Color.Green, 0.3f), (isSelected == true) ? 12 : 8),
                        MaxVisible = _maxVisibleTargetStyle,
                    };
                }

                if (gf.Geometry is Polygon || gf.Geometry is MultiPolygon)
                {
                    return new VectorStyle
                    {
                        Fill = null,
                        Outline = new Pen(Color.Opacity(Color.Green, 0.3f), (isSelected == true) ? 12 : 8),
                        Line = null,
                        MaxVisible = _maxVisibleTargetStyle,
                    };
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
            // TODO: fix this
            var isSelected = (bool?)f[SelectField] ?? false;

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
                    // TODO: what's it?
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
                return new SymbolStyle
                {
                    Fill = new Brush(Color.Opacity(Color.Black, 0.25f)),
                    Outline = new Pen(Color.Black, (isSelected == true) ? 4.0 : 1.0),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = (isSelected == true) ? 0.6 : 0.4,
                    MaxVisible = _maxVisibleTargetStyle,
                };
            }

            if (gf.Geometry is LineString || gf.Geometry is MultiLineString)
            {
                style.Fill = null;
                style.Line = new Pen(Color.Black, (isSelected == true) ? 4 : 2);
            }

            if (gf.Geometry is Polygon || gf.Geometry is MultiPolygon)
            {
                style.Fill = new Brush(Color.Opacity(Color.Black, 0.25f));
                style.Outline = new Pen(Color.Black, (isSelected == true) ? 4 : 1);
                style.Line = null;
            }

            return style;
        });
    }

    private static IStyle CreateSensorStyle(IPalette? palette)
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

            if ((palette is IColorPalette colorPalette) == false)
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

                        var color = colorPalette.PickColor(name);

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

    private static IStyle CreateTrackStyle(IPalette? palette)
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

            if ((palette is IColorPalette colorPalette) == false)
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

                        var color = colorPalette.PickColor(name);

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

    private static IStyle CreateEditLayerStyle(IPalette? _)
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

    private static IStyle CreateUserLayerStyle(IPalette? _)
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
                    if (item.Equals(SelectField) == true)
                    {
                        var isSelect = (bool)gf[SelectField]!;

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

    private static IStyle CreateFootprintImageBorderStyle(IPalette? palette)
    {
        if ((palette is MapsuiPalette mapsuiPalette) == false)
        {
            return new ThemeStyle(f => null);
        }

        return new VectorStyle
        {
            Fill = new Brush(Color.Opacity(mapsuiPalette.Color, 0.25f)),
            Line = new Pen(mapsuiPalette.Color, 1.0),
            Outline = new Pen(mapsuiPalette.Color, 1.0),
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
                        if (string.Equals((string)gf["Name"]!, ExtraPolygonHoverLineField) == true)
                        {
                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(_color, 4) { PenStyle = PenStyle.Dot },
                            };
                        }
                        else if (string.Equals((string)gf["Name"]!, ExtraPolygonAreaField) == true)
                        {
                            return new VectorStyle
                            {
                                Fill = new Brush(Color.Opacity(_color, 0.25f)),
                                Line = null,
                                Outline = null,
                            };
                        }
                        else if (string.Equals((string)gf["Name"]!, ExtraRouteHoverLineField) == true)
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

    //private static IStyle CreateGroundStationLayerStyle()
    //{
    //    return new ThemeStyle(f =>
    //    {
    //        if (f is not GeometryFeature gf)
    //        {
    //            return null;
    //        }

    //        if (gf.Geometry is Point)
    //        {
    //            return null;
    //        }

    //        if (gf.Geometry is MultiPolygon)
    //        {
    //            foreach (var item in gf.Fields)
    //            {
    //                if (item.Equals("Count"))
    //                {
    //                    var count = int.Parse(gf["Count"]!.ToString()!);
    //                    var index = int.Parse(gf["Index"]!.ToString()!);
    //                    var color = _groundStationPalette.GetColor(index, count);
    //                    return new VectorStyle
    //                    {
    //                        Fill = new Brush(Color.Opacity(new Color(color.R, color.G, color.B), 0.45f)),
    //                        Line = null,
    //                        Outline = null,
    //                        Enabled = true
    //                    };
    //                }
    //            }
    //        }

    //        if (gf.Fields != null && gf.Geometry is MultiLineString)
    //        {
    //            foreach (var item in gf.Fields)
    //            {
    //                if (item.Equals("InnerBorder"))
    //                {
    //                    var count = int.Parse(gf["Count"]!.ToString()!);
    //                    var color = _groundStationPalette.GetColor(0, count);

    //                    return new VectorStyle
    //                    {
    //                        Fill = null,
    //                        Line = new Pen(new Color(color.R, color.G, color.B), 2.0),
    //                        Outline = new Pen(new Color(color.R, color.G, color.B), 2.0),
    //                        Enabled = true
    //                    };
    //                }
    //                if (item.Equals("OuterBorder"))
    //                {
    //                    var count = int.Parse(gf["Count"]!.ToString()!);
    //                    var color = _groundStationPalette.GetColor(count - 1, count);

    //                    return new VectorStyle
    //                    {
    //                        Fill = null,
    //                        Line = new Pen(new Color(color.R, color.G, color.B), 2.0),
    //                        Outline = new Pen(new Color(color.R, color.G, color.B), 2.0),
    //                        Enabled = true
    //                    };
    //                }
    //            }
    //        }

    //        return null;
    //    });
    //}

    private static IStyle CreateGroundStationLayerStyle(IPalette? palette)
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

            if ((palette is ISingleHuePalette singleHuePalette) == false)
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
                        var color = singleHuePalette.GetColor(index, count);
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
                        var color = singleHuePalette.GetColor(0, count);

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
                        var color = singleHuePalette.GetColor(count - 1, count);

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
