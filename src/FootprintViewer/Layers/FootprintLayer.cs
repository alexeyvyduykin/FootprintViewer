using Mapsui;
using Mapsui.Layers;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public class FootprintLayer : BaseCustomLayer<ILayer>
    {
        public FootprintLayer(ILayer source) : base(source)
        {

        }

        public int MaxVisiblePreview { get; set; }

        public override IEnumerable<IFeature> GetFeatures(MRect box, double resolution)
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
                return base.GetFeatures(box, resolution);
            }

            return new List<IFeature>();
        }
    }
}
