using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomFootprintDataSource : IDataSource<Footprint>
    {
        private List<Footprint>? _footprints;
        private readonly IDataSource<Satellite> _source;

        public RandomFootprintDataSource(IDataSource<Satellite> source)
        {
            _source = source;
        }

        public async Task<List<Footprint>> GetNativeValuesAsync(IFilter<Footprint>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _source.GetNativeValuesAsync(null);

                    _footprints = FootprintBuilder.Create(satellites).ToList();
                }

                return _footprints;
            });
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<Footprint, T> converter)
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _source.GetNativeValuesAsync(null);

                    _footprints = FootprintBuilder.Create(satellites).ToList();
                }

                return _footprints.Select(s => converter(s)).ToList();
            });
        }
    }
}
