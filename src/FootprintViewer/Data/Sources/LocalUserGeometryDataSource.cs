using System.Collections.Generic;
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

        public IEnumerable<UserGeometry> GetUserGeometries() => _userGeometries;
    }
}
