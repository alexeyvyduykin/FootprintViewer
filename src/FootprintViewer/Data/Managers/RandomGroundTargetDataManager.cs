using FootprintViewer.Data.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class RandomGroundTargetDataManager : BaseDataManager<GroundTarget, IRandomSource>
    {
        private List<GroundTarget>? _groundTargets;

        protected override async Task<List<GroundTarget>> GetNativeValuesAsync(IRandomSource dataSource, IFilter<GroundTarget>? filter)
        {
            return await Task.Run(async () =>
            {
                _groundTargets ??= await Build(dataSource);

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
                _groundTargets ??= await Build(dataSource);

                if (filter == null)
                {
                    return _groundTargets.Select(s => converter(s)).ToList();
                }

                return _groundTargets.Select(s => converter(s)).Where(s => filter.Filtering(s)).ToList();
            });
        }

        private static async Task<List<GroundTarget>> Build(IRandomSource dataSource)
        {
            var manager = (IDataManager<Footprint>)new RandomFootprintDataManager();

            var footprints = await manager.GetNativeValuesAsync(new RandomSource() { GenerateCount = dataSource.GenerateCount }, null);

            return GroundTargetBuilder.Create(footprints).ToList();
        }
    }
}
