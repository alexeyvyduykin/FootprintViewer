using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomSatelliteDataSource : BaseRandomSource, IDataSource<Satellite>
    {
        private List<Satellite>? _satellites;

        public async Task<List<Satellite>> GetNativeValuesAsync(IFilter<Satellite>? filter)
        {
            return await Task.Run(() =>
            {
                return _satellites ??= new List<Satellite>(SatelliteBuilder.Create());
            });
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<Satellite, T> converter)
        {
            return await Task.Run(() =>
            {
                _satellites ??= new List<Satellite>(SatelliteBuilder.Create());

                return _satellites.Select(s => converter(s)).ToList();
            });
        }
    }
}
