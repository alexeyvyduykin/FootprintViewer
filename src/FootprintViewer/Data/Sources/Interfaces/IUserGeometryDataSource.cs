using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public interface IUserGeometryDataSource
    {
        Task AddAsync(UserGeometry geometry);

        Task RemoveAsync(UserGeometry geometry);

        Task<List<UserGeometry>> GetUserGeometriesAsync();

        Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync();

        Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(string[] names);

        Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry);
    }
}
