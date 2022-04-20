using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class VertexOnlyLayer : BaseCustomLayer
    {
        public VertexOnlyLayer(ILayer source) : base(source) { }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
        {
            var features = Source.GetFeatures(box, resolution).ToList();

            foreach (var feature in features)
            {
                if (feature is GeometryFeature gf)
                {
                    if (gf.Geometry is Point || gf.Geometry is MultiPoint)
                    {
                        throw new Exception();
                    }

                    if (feature is InteractiveFeature interactiveFeature)
                    {
                        foreach (var point in interactiveFeature.EditVertices())
                        {
                            yield return new GeometryFeature { Geometry = point.ToPoint() };
                        }
                    }
                }
            }
        }
    }
}
