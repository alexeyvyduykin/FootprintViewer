using FootprintViewer.Data;
using System.Collections.Generic;

namespace FootprintViewer.Layers
{
    public interface IFootprintLayerSource : ILayerSource
    {
        void UpdateData(List<Footprint> footprints);
    }

    public class FootprintLayerSource : BaseLayerSource<Footprint>, IFootprintLayerSource
    {
        public void UpdateData(List<Footprint> footprints)
        {
            Clear();
            AddRange(FeatureBuilder.Build(footprints));
            DataHasChanged();
        }
    }
}
