using FootprintViewer.Data;
using Mapsui.Layers;
using Mapsui.Providers;

namespace FootprintViewer.Layers
{
    public class CustomProvider : WritableLayer
    {
        private readonly UserGeometryProvider _provider;

        public CustomProvider(UserGeometryProvider provider)
        {
            _provider = provider;
        }

        public void AddFeature(IFeature feature)
        {
            Add(feature);
        }
    }
}
