using FootprintViewer.ViewModels;
using Mapsui.Layers;

namespace FootprintViewer.Layers
{
    public class TrackLayer : MemoryLayer
    {         
        private readonly TrackLayerProvider _provider;

        public TrackLayer(TrackLayerProvider provider)
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
