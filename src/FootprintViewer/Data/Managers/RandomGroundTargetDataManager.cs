using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class RandomGroundTargetDataManager : BaseDataManager<GroundTarget, IRandomSource>
    {
        private List<GroundTarget>? _groundTargets;
        private readonly IDataManager<Footprint> _manager;

        public RandomGroundTargetDataManager(IDataManager<Footprint> manager)
        {
            _manager = manager;
        }

        protected override async Task<List<GroundTarget>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<GroundTarget>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _manager.GetNativeValuesAsync(null!, null);

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

        protected override async Task<List<T>> GetValuesAsync<T>(IRandomSource dataSource, IFilter<T>? filter, Func<GroundTarget, T> converter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _manager.GetNativeValuesAsync(null!, null);

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
