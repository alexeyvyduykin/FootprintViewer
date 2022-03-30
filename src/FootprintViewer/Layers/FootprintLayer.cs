using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class FootprintLayer : BaseCustomLayer
    {
        public FootprintLayer(ILayer source) : base(source) { }

        public int MaxVisiblePreview { get; set; }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            // preview
            //if (resolution >= MaxVisiblePreview)
            //{
            //    foreach (var feature in _source.GetFeaturesInView(box, resolution))
            //    {
            //        var center = feature.Geometry.BoundingBox.Centroid;

            //        yield return new Feature()
            //        {
            //            ["Name"] = feature["Name"],
            //            ["State"] = feature["State"],
            //            Geometry = center,
            //        };
            //    }
            //}

            if (resolution < MaxVisiblePreview)
            {
                foreach (var feature in Source.GetFeaturesInView(box, resolution))
                {
                    yield return feature;
                }
            }
        }
    }
}
