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

        public void Remove(UserGeometry geometry)
        {
            _userGeometries.Remove(geometry);
        }

        public async Task<List<UserGeometry>> GetUserGeometriesAsync() => await Task.Run(() => _userGeometries);

        public async Task<List<UserGeometryInfo>> GetUserGeometryInfosAsync() => await Task.Run(() => _userGeometries.Select(s => new UserGeometryInfo(s)).ToList());

        public async Task RemoveAsync(UserGeometry geometry)
        {
            throw new System.NotImplementedException();
        }
    }
}
