using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class RandomSatelliteDataManager : BaseDataManager<Satellite, IRandomSource>
    {
        private List<Satellite>? _satellites;

        protected override async Task<List<Satellite>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<Satellite>? filter)
        {
            return await Task.Run(() =>
            {
                return _satellites ??= new List<Satellite>(SatelliteBuilder.Create());
            });
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IRandomSource dataSource, IFilter<T>? filter, Func<Satellite, T> converter)
        {
            return await Task.Run(() =>
            {
                _satellites ??= new List<Satellite>(SatelliteBuilder.Create());

                return _satellites.Select(s => converter(s)).ToList();
            });
        }
    }
}
