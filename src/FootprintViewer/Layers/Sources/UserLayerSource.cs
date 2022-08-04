using FootprintViewer.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.Layers
{
    public interface IUserLayerSource : ILayerSource
    {
        void UpdateData(List<UserGeometry> userGeometries);
    }

    public class UserLayerSource : BaseLayerSource<UserGeometry>, IUserLayerSource
    {
        public void UpdateData(List<UserGeometry> userGeometries)
        {
            var arr = userGeometries
                .Where(s => s.Geometry != null)
                .Select(s => s.Geometry!.ToFeature(s.Name!));

            Clear();
            AddRange(arr);
            DataHasChanged();
        }
    }
}
