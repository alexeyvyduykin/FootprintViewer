using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Data.Sources
{
    public class RandomGroundTargetDataSource : IGroundTargetDataSource
    {
        private readonly IEnumerable<GroundTarget> _groundTargets;

        public RandomGroundTargetDataSource(IFootprintDataSource footprintDataSource)
        {
            var footprints = footprintDataSource.GetFootprints().ToList();
            _groundTargets = GroundTargetBuilder.Create(footprints);
        }

        public IEnumerable<GroundTarget> GetGroundTargets() => _groundTargets;
    }
}
