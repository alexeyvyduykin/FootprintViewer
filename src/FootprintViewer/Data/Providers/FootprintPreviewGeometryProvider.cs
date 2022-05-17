using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data
{
    public class FootprintPreviewGeometryProvider : BaseProvider<IDataSource<(string, Geometry)>>
    {
        public async Task<List<(string, Geometry)>> GetValuesAsync()
        {
            return await Task.Run(async () =>
            {
                var list = new List<(string, Geometry)>();

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
