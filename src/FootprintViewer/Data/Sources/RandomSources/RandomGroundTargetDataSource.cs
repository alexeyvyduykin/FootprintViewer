using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IDataSource<GroundTarget>
    {
        private List<GroundTarget>? _groundTargets;
        private readonly IDataSource<Footprint> _source;

        public RandomGroundTargetDataSource(IDataSource<Footprint> source)
        {
            _source = source;
        }

        public async Task<List<GroundTarget>> GetNativeValuesAsync(IFilter<GroundTarget>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetNativeValuesAsync(null);

                    var targets = GroundTargetBuilder.Create(footprints);

                    _groundTargets = new List<GroundTarget>(targets);
                }

                if (filter == null)
                {
                    return _groundTargets;
                }

                return _groundTargets.Where(s => filter.Filtering(s)).ToList();
            });
        }

        public async Task<List<T>> GetValuesAsync<T>(IFilter<T>? filter, Func<GroundTarget, T> converter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetNativeValuesAsync(null);

                    var targets = GroundTargetBuilder.Create(footprints);

                    _groundTargets = new List<GroundTarget>(targets);
                }

                if (filter == null)
                {
                    return _groundTargets.Select(s => converter(s)).ToList();
                }

                return _groundTargets.Select(s => converter(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
