using FootprintViewer.Data.Sources;
using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public class FootprintProvider : BaseProvider<IFootprintDataSource>
    {
        public IEnumerable<Footprint> GetFootprints()
        {
            var list = new List<Footprint>();

            foreach (var source in Sources)
            {
                list.AddRange(source.GetFootprints());
            }

            return list;
        }
    }
}
