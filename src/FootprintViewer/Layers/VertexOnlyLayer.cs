using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using FootprintViewer.InteractivityEx;

namespace FootprintViewer.Layers
{
    public class VertexOnlyLayer : BaseLayer
    {
        private readonly ILayer _source;

        private static readonly SymbolStyle PointStyle = new SymbolStyle
        {
            Fill = new Brush(Color.White),
            Outline = new Pen(Color.Black, 2 / 0.3),
            SymbolType = SymbolType.Ellipse,
            SymbolScale = 0.3,       
        };
       
        public override BoundingBox Envelope => _source.Envelope;

        public VertexOnlyLayer(ILayer source)
        {
            _source = source;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
            Style = PointStyle;
        }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var features = _source.GetFeaturesInView(box, resolution).ToList();
            foreach (var feature in features)
            {
                if (feature.Geometry is Point || feature.Geometry is MultiPoint)
                {
                    throw new Exception();
                }

                if (feature is InteractiveFeature interactiveFeature)
                {
                    foreach (var point in interactiveFeature.EditVertices())
                    {                          
                        yield return new Feature { Geometry = point };
                    }
                }
            }
        }

        public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
