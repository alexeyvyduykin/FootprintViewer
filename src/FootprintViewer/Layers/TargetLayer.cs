using FootprintViewer.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FootprintViewer.Layers
{
    public class TargetLayerEventArgs : EventArgs
    {
        public IEnumerable<IFeature>? Features { get; set; }
        public double Resolution { get; set; }
    }

    public delegate void TargetLayerEventHandler(object? sender, TargetLayerEventArgs e);

    public class TargetLayer : MemoryLayer
    {
        private IStyle _style;
        private TargetProvider _provider;
        private const int _maxVisible = 5000;

        public TargetLayer(IProvider provider)
        {
            _provider = (TargetProvider)provider;
            _style = CreateTargetThemeStyle();
            
            Name = nameof(LayerType.GroundTarget);
            Style = _style;
            DataSource = provider;
            IsMapInfoLayer = false;
        }

        public event TargetLayerEventHandler OnRefreshData;

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            base.RefreshData(extent, resolution, changeType);

            if (changeType == ChangeType.Discrete)
            {
                if (resolution < _maxVisible && extent.Equals(new BoundingBox(0,0,0,0)) == false)
                {
                    OnRefreshData?.Invoke(this, new TargetLayerEventArgs()
                    {
                        Resolution = resolution,
                        Features = GetFeaturesInView(extent, resolution),
                    });
                }
                else
                {
                    OnRefreshData?.Invoke(this, new TargetLayerEventArgs()
                    {
                        Resolution = resolution,
                        Features = null
                    });
                }
            }
        }

        public IEnumerable<GroundTarget> GetTargets(IEnumerable<IFeature> features)
        {
            return _provider.FromDataSource(features);
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
                        MaxVisible = _maxVisible,
                    };
                }

                if (f.Geometry is LineString || f.Geometry is MultiLineString)
                {
                    return new VectorStyle
                    {
                        Fill = null,
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = _maxVisible,
                    };
                }

                if (f.Geometry is Polygon || f.Geometry is MultiPolygon)
                {
                    return new VectorStyle
                    {
                        Fill = null,// new Brush(Color.Opacity(Color.Black, 0.3f)),
                        Outline = new Pen(Color.Black, 2),
                        Line = new Pen(Color.Black, 2),
                        MaxVisible = _maxVisible,
                    };
                }

                throw new Exception();
            });
        }
    }
}
