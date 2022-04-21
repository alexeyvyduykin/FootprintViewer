using Mapsui;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Styles;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace InteractiveSample.Layers
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

        public VertexOnlyLayer(ILayer source)
        {
            _source = source;
            _source.DataChanged += (sender, args) => OnDataChanged(args);
            Style = PointStyle;
        }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            var features = _source.GetFeatures(box, resolution).ToList();
            foreach (var feature in features)
            {
                if (feature is GeometryFeature gf)
                {
                    if (gf.Geometry is Point || gf.Geometry is MultiPoint)
                    {
                        continue;
                    }

                    if (gf.Geometry != null)
                    {
                        foreach (var vertex in gf.Geometry.Coordinates)
                        {
                            yield return vertex.ToPoint().ToFeature();
                        }
                    }
                }
            }
        }

        public override void RefreshData(FetchInfo fetchInfo)
        {
            OnDataChanged(new DataChangedEventArgs());
        }
    }
}
