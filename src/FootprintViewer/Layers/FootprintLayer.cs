using FootprintViewer.Data;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Layers
{
    public class FootprintLayer : MemoryLayer
    {
        private readonly FootprintLayerProvider _provider;
    
        public FootprintLayer(FootprintLayerProvider provider)
        {
            _provider = provider;
     
            DataSource = _provider;
        }

        public Footprint GetFootprint(string name) => _provider.GetFootprint(name);

        public void SelectFeature(string name) => _provider.SelectFeature(name);

        public void UnselectFeature(string name) => _provider.UnselectFeature(name);

        public bool IsSelect(string name) => _provider.IsSelect(name);

        public Task<List<Footprint>> GetFootprintsAsync() => _provider.GetFootprintsAsync();

        public List<Footprint> GetFootprints() => _provider.GetFootprints();

        public int MaxVisiblePreview { get; set; }

        public override IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            if (resolution >= MaxVisiblePreview) // preview
            {             
                foreach (var feature in base.GetFeaturesInView(box, resolution))
                {
                    var center = feature.Geometry.BoundingBox.Centroid;

                    yield return new Feature()
                    {
                        ["Name"] = feature["Name"],
                        ["State"] = feature["State"],
                        Geometry = center,
                    };
                }
            }
            else
            {         
                foreach (var feature in base.GetFeaturesInView(box, resolution))
                {
                    yield return feature;
                }
            }
        }
    }
}
