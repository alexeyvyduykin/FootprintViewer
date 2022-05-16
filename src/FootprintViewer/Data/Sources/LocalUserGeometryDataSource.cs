using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class LocalUserGeometryDataSource : IUserGeometryDataSource
    {
        private readonly List<UserGeometry> _userGeometries;

        public LocalUserGeometryDataSource()
        {
            _userGeometries = new List<UserGeometry>();
        }

        public async Task AddAsync(UserGeometry geometry)
        {
            await Task.Run(() => { _userGeometries.Add(geometry); });
        }

        public async Task RemoveAsync(UserGeometry geometry)
        {
            await Task.Run(() => { _userGeometries.Remove(geometry); });
        }

        public async Task UpdateGeometry(string key, NetTopologySuite.Geometries.Geometry geometry)
        {
            await Task.Run(() =>
            {
                var userGeometry = _userGeometries.Where(s => s.Name == key).FirstOrDefault();

                if (userGeometry != null)
                {
                    userGeometry.Geometry = geometry;
                }
            });
        }

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync(IFilter<UserGeometryInfo>? filter)
        {
            return await Task.Run(() =>
            {
                if (filter == null || filter.Names == null)
                {
                    return _userGeometries.Select(s => new UserGeometryInfo(s)).ToList();
                }

                return _userGeometries.Select(s => new UserGeometryInfo(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
