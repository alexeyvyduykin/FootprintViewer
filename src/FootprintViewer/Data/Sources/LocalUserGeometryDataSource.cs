using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class LocalUserGeometryDataSource : IEditableDataSource<UserGeometry>
    {
        private readonly List<UserGeometry> _userGeometries;

        public LocalUserGeometryDataSource()
        {
            _userGeometries = new List<UserGeometry>();
        }

        public async Task AddAsync(UserGeometry value)
        {
            await Task.Run(() => { _userGeometries.Add(value); });
        }

        public async Task RemoveAsync(UserGeometry value)
        {
            await Task.Run(() => { _userGeometries.Remove(value); });
        }

        public async Task EditAsync(string key, UserGeometry value)
        {
            await Task.Run(() =>
            {
                var userGeometry = _userGeometries.Where(s => s.Name == key).FirstOrDefault();

                if (userGeometry != null)
                {
                    userGeometry.Geometry = value.Geometry;
                }
            });
        }

        public async Task<List<UserGeometry>> GetNativeValuesAsync(IFilter<UserGeometry>? filter)
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

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<UserGeometry, T> converter)
        {
            return await Task.Run(() =>
            {
                if (filter == null || filter.Names == null)
                {
                    return _userGeometries.Select(s => converter(s)).ToList();
                }

                return _userGeometries.Select(s => converter(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
