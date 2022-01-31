using FootprintViewer.Data.Sources;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class MapProvider : BaseProvider<IMapDataSource>
    {
        public IEnumerable<MapResource> GetMapResources()
        {
            var list = new List<MapResource>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetMapResources());
            }

            return list;
        }
    }
}
