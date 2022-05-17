using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class MapProvider : BaseProvider<IDataSource<MapResource>>
    {
        public async Task<List<MapResource>> GetValuesAsync()
        {
            return await Task.Run(async () =>
            {
                var list = new List<MapResource>();

                foreach (var source in Sources)
                {
                    var values = await source.GetValuesAsync(null);
                    list.AddRange(values);
                }

                return list;
            });
        }
    }
}
