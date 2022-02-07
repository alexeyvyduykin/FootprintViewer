using FootprintViewer.ViewModels;
using Mapsui.Layers;

namespace FootprintViewer.Layers
{
    public class SensorLayer : MemoryLayer
    {
        private readonly SensorLayerProvider _provider;

        public SensorLayer(SensorLayerProvider provider)
        {
            _provider = provider;

            DataSource = provider;
        }

        public void Update(SatelliteInfo info)
        {
            _provider.Update(info);
            DataHasChanged();
        }
    }
}
