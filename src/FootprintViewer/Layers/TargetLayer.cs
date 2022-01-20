using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public class TargetLayerEventArgs : EventArgs
    {
        public IEnumerable<IFeature>? Features { get; set; }
        public double Resolution { get; set; }
    }

    public delegate void TargetLayerEventHandler(object? sender, TargetLayerEventArgs e);

    public class TargetLayer : MemoryLayer, IGroundTargetDataSource
    {
        private readonly IStyle _style;
        private readonly TargetProvider _provider;
        private const int _maxVisible = 5000;
        private BoundingBox _lastExtent = new BoundingBox(1, 1, 1, 1);
        private readonly ReactiveCommand<IEnumerable<IFeature>?, IEnumerable<IFeature>?> refresh;

        public TargetLayer(IProvider provider)
        {
            _provider = (TargetProvider)provider;

            var style1 = CreateTargetHighlightThemeStyle();
            var style2 = CreateTargetThemeStyle();

            _style = new StyleCollection() { style1, style2 };
            Style = _style;

            Name = nameof(LayerType.GroundTarget);

            DataSource = provider;
            IsMapInfoLayer = false;

            refresh = ReactiveCommand.Create<IEnumerable<IFeature>?, IEnumerable<IFeature>?>(s => s);
        }

        public IObservable<IEnumerable<IFeature>?> RefreshDataObservable => refresh;

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            base.RefreshData(extent, resolution, changeType);

            if (changeType == ChangeType.Discrete)
            {
                if (extent.Left != _lastExtent.Left && extent.Top != _lastExtent.Top && extent.Right != _lastExtent.Right && extent.Bottom != _lastExtent.Bottom)
                {
                    if (resolution < _maxVisible && extent.Equals(new BoundingBox(0, 0, 0, 0)) == false)
                    {
                        // HACK: change size extent to viewport of view control
                        var box = extent.Grow(-SymbolStyle.DefaultWidth * 2 * resolution, -SymbolStyle.DefaultHeight * 2 * resolution);

                        refresh.Execute(GetFeaturesInView(box, resolution)).Subscribe();
                    }
                    else
                    {
                        refresh.Execute(null).Subscribe();
                    }

                    _lastExtent = extent.Copy();
                }
            }
        }

        public void SelectGroundTarget(string name)
        {
            _provider.SelectFeature(name);
            DataHasChanged();
        }

        public void ShowHighlight(string name)
        {
            _provider.ShowHighlight(name);
            DataHasChanged();
        }

        public void HideHighlight()
        {
            _provider.HideHighlight();
            DataHasChanged();
        }

        public IEnumerable<GroundTarget> GetTargets(IEnumerable<IFeature> features)
        {
            return _provider.FromDataSource(features);
        }

        public IEnumerable<GroundTarget> GetTargets()
        {
            return _provider.FromDataSource();
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
                                    MaxVisible = _maxVisible,
                                };
                            case "Selected":
                                return new SymbolStyle
                                {
                                    Fill = new Brush(Color.Opacity(Color.Green, 0.4f)),
                                    Outline = null,
                                    SymbolType = SymbolType.Ellipse,
                                    SymbolScale = 0.8,
                                    MaxVisible = _maxVisible,
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
                                    MaxVisible = _maxVisible,
                                };
                            case "Selected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Line = new Pen(Color.Opacity(Color.Green, 0.3f), 12),
                                    MaxVisible = _maxVisible,
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
                                    MaxVisible = _maxVisible,
                                };
                            case "Selected":
                                return new VectorStyle
                                {
                                    Fill = null,
                                    Outline = new Pen(Color.Opacity(Color.Green, 0.3f), 12),
                                    Line = null,
                                    MaxVisible = _maxVisible,
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
                                MaxVisible = _maxVisible,
                            };
                        case "Selected":
                            return new SymbolStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 4.0),
                                SymbolType = SymbolType.Ellipse,
                                SymbolScale = 0.6,
                                MaxVisible = _maxVisible,
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
                                MaxVisible = _maxVisible,
                            };
                        case "Selected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Line = new Pen(Color.Black, 4),
                                MaxVisible = _maxVisible,
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
                                MaxVisible = _maxVisible,
                            };
                        case "Selected":
                            return new VectorStyle
                            {
                                Fill = null,
                                Outline = new Pen(Color.Black, 4),
                                Line = null,
                                MaxVisible = _maxVisible,
                            };
                        default:
                            break;
                    }
                }

                throw new Exception();
            });
        }
    }
}
