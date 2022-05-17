using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class LocalUserGeometryDataSource : IEditableDataSource<UserGeometryInfo>
    {
        private readonly List<UserGeometryInfo> _userGeometries;

        public LocalUserGeometryDataSource()
        {
            _userGeometries = new List<UserGeometryInfo>();
        }

        public async Task AddAsync(UserGeometryInfo value)
        {
            await Task.Run(() => { _userGeometries.Add(value); });
        }

        public async Task RemoveAsync(UserGeometryInfo value)
        {
            await Task.Run(() => { _userGeometries.Remove(value); });
        }

        public async Task EditAsync(string key, UserGeometryInfo value)
        {
            await Task.Run(() =>
            {
                var userGeometry = _userGeometries.Where(s => s.Name == key).FirstOrDefault();

                if (userGeometry != null)
                {
                    userGeometry.Geometry.Geometry = value.Geometry.Geometry;
                }
            });
        }

        public async Task<List<UserGeometryInfo>> GetValuesAsync(IFilter<UserGeometryInfo>? filter)
        {
            return await Task.Run(() =>
            {
                if (filter == null || filter.Names == null)
                {
                    return _userGeometries.ToList();
                }

                return _userGeometries.Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
