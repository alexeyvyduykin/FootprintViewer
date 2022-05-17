using FootprintViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private List<GroundTargetInfo>? _groundTargets;
        private readonly IDataSource<FootprintInfo> _source;

        public RandomGroundTargetDataSource(IDataSource<FootprintInfo> source)
        {
            _source = source;
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfosAsync(IFilter<GroundTargetInfo>? filter)
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetValuesAsync(null);

                    var targets = GroundTargetBuilder.Create(footprints.Select(s => s.Footprint!));

                    _groundTargets = new List<GroundTargetInfo>(targets.Select(s => new GroundTargetInfo(s)));
                }

                if (filter == null)
                {
                    return _groundTargets;
                }

                return _groundTargets.Where(s => filter.Filtering(s)).ToList();
            });
        }
    }
}
