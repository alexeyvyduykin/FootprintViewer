using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private List<GroundTarget>? _groundTargets;
        private readonly IFootprintDataSource _source;

        public RandomGroundTargetDataSource(IFootprintDataSource source)
        {
            _source = source;
        }

        public async Task<List<GroundTarget>> GetGroundTargetsAsync()
        {
            return await Task.Run(async () =>
            {
                if (_groundTargets == null)
                {
                    var footprints = await _source.GetFootprintsAsync();

                    _groundTargets = new List<GroundTarget>(GroundTargetBuilder.Create(footprints));
                }

                return _groundTargets;
            });
        }
    }
}
