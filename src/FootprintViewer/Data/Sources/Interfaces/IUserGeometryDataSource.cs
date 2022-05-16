using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IUserGeometryDataSource
    {
        Task AddAsync(UserGeometry geometry);

        Task RemoveAsync(UserGeometry geometry);

        Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(IFilter<UserGeometryInfo>? filter);

        Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry);
    }
}
