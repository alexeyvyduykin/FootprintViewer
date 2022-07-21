using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class RandomFootprintDataManager : BaseDataManager<Footprint, IRandomSource>
    {
        private List<Footprint>? _footprints;
        private readonly IDataManager<Satellite> _manager;

        public RandomFootprintDataManager(IDataManager<Satellite> manager)
        {
            _manager = manager;
        }

        protected override async Task<List<Footprint>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<Footprint>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _manager.GetNativeValuesAsync(null!, null);

                    _footprints = FootprintBuilder.Create(satellites).ToList();
                }

                return _footprints;
            });
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IRandomSource dataSource, IFilter<T>? filter, Func<Footprint, T> converter)
        {
            return await Task.Run(async () =>
            {
                if (_footprints == null)
                {
                    var satellites = await _manager.GetNativeValuesAsync(null!, null);

                    _footprints = FootprintBuilder.Create(satellites).ToList();
                }

                return _footprints.Select(s => converter(s)).ToList();
            });
        }
    }
}
