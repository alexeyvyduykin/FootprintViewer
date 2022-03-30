using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Layers
{
    public class VertexOnlyLayer : BaseCustomLayer
    {
        public VertexOnlyLayer(ILayer source) : base(source) { }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var features = Source.GetFeaturesInView(box, resolution).ToList();

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
    }

    //public class VertexOnlyLayer : BaseLayer
    //{
    //    private readonly ILayer _source;

    //    public override BoundingBox Envelope => _source.Envelope;

    //    public VertexOnlyLayer(ILayer source)
    //    {
    //        _source = source;
    //        _source.DataChanged += (sender, args) => OnDataChanged(args);
    //    }

    //    public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
    //    {
    //        var features = _source.GetFeaturesInView(box, resolution).ToList();
    //        foreach (var feature in features)
    //        {
    //            if (feature.Geometry is Point || feature.Geometry is MultiPoint)
    //            {
    //                throw new Exception();
    //            }

    //            if (feature is InteractiveFeature interactiveFeature)
    //            {
    //                foreach (var point in interactiveFeature.EditVertices())
    //                {
    //                    yield return new Feature { Geometry = point };
    //                }
    //            }
    //        }
    //    }

    //    public override void RefreshData(BoundingBox extent, double resolution, ChangeType changeType)
    //    {
    //        OnDataChanged(new DataChangedEventArgs());
    //    }
    //}
}
