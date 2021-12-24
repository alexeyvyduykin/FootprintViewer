using InteractivitySample.Interactivity.Decorators;
using InteractivitySample.Interactivity.Designers;
using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.Generic;

namespace InteractivitySample.Interactivity
{
    public class InteractiveLayer : BaseLayer
    {
        private readonly ILayer _source;
        private readonly IInteractiveObject? _interactiveObject;
        private static readonly Color _color = new Color(76, 154, 231);
        private static readonly Color _darkColor = new Color(67, 135, 202);

        public override BoundingBox Envelope => _source.Envelope;

        public InteractiveLayer(ILayer source, IInteractiveObject interactiveObject)
        {
            _source = source;
            _interactiveObject = interactiveObject;
            _source.DataChanged += (sender, args) => OnDataChanged(args);

            _interactiveObject.InvalidateLayer += (s, e) => DataHasChanged();

            Style = CreateDefaultStyle(interactiveObject);

            IsMapInfoLayer = true;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (_interactiveObject is IDecorator decorator)
            {
                var feature = decorator.FeatureSource;

                if (feature == null)
                {
                    yield break;
                }

                if (box.Intersects(feature.Geometry.BoundingBox) == true)
                {
                    foreach (var point in decorator.GetActiveVertices())
                    {
                        yield return new Feature { Geometry = point };
                    }
                }
            }
            else if (_interactiveObject is IDesigner designer)
            {
                var feature = designer.Feature;

                yield return feature;

                if (designer.ExtraFeatures.Count != 0)
                {
                    foreach (var item in designer.ExtraFeatures)
                    {
                        yield return item;
                    }
                }

                foreach (var point in designer.GetActiveVertices())
                {
                    yield return new Feature { Geometry = point };
                }
            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
        
        private static IStyle CreateDefaultStyle(IInteractiveObject interactiveObject)
        {
            // To show the selected style a ThemeStyle is used which switches on and off the SelectedStyle
            // depending on a "Selected" attribute.
            return new ThemeStyle(f =>
            {
                if (f.Geometry is Point)
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

                if (interactiveObject is IDesigner)
                {
                    if (f.Fields != null)
                    {
                        foreach (var item in f.Fields)
                        {
                            if (item.Equals("Name") == true)
                            {
                                if ((string)f["Name"] == "ExtraPolygonHoverLine")
                                {
                                    return new VectorStyle
                                    {
                                        Fill = null,
                                        Line = new Pen(_color, 4) { PenStyle = PenStyle.Dot },
                                    };
                                }
                                else if ((string)f["Name"] == "ExtraPolygonArea")
                                {
                                    return new VectorStyle
                                    {
                                        Fill = new Brush(Color.Opacity(_color, 0.25f)),
                                        Line = null,
                                        Outline = null,
                                    };
                                }
                                else if ((string)f["Name"] == "ExtraRouteHoverLine")
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

                    if (f.Geometry is Polygon || f.Geometry is LineString)
                    {
                        return new VectorStyle
                        {
                            Fill = new Brush(Color.Transparent),
                            Line = new Pen(_color, 3),
                            Outline = new Pen(_color, 3)
                        };
                    }
                }

                return null;
            });
        }
    }
}
